using NLog;
using System.Threading.Tasks;

namespace VideoAudioProcessor;

class Worker
{
    private readonly Logger _logs;
    private readonly Config _config;
    
    private RabbitMQPublisher? _publisher;
    private CameraStreamHandler? _cameraStream;
    private VideoFrameProcessor? _videoProcessor;
    private YandexSpeechRecognizer? _speechRecognizer;
    private AudioPreProcessor? _audioPreProcessor;
    
    private bool _started = false;
    private DateTime _lastVideoFrameTime = DateTime.MinValue;
    private readonly TimeSpan _videoFrameInterval;
    
    // Statistics tracking (thread-safe using Interlocked)
    private long _totalAudioFrames = 0;
    private long _filteredAudioFrames = 0;
    private long _apiCalls = 0;

    public Worker(Logger log, Config? config)
    {
        _logs = log;
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _videoFrameInterval = TimeSpan.FromSeconds(1.0 / _config.VideoFps);
    }

    public async Task InitializeAsync()
    {
        // Initialize RabbitMQ publisher
        _publisher = new RabbitMQPublisher(_logs, _config);
        var connected = await _publisher.ConnectAsync();
        if (!connected)
        {
            _logs.Error("Failed to connect to RabbitMQ");
            return;
        }
        _logs.Info("RabbitMQ publisher connected");

        // Initialize video processor
        _videoProcessor = new VideoFrameProcessor(_logs, _config.VideoWidth, _config.JpegQuality);
        _logs.Info("Video processor initialized");

        // Initialize audio pre-processor (VAD, silence/noise filtering)
        _audioPreProcessor = new AudioPreProcessor(_logs, _config);
        _audioPreProcessor.SpeechDetected += (s, data) => _ = OnSpeechDetectedAsync(s, data);
        _audioPreProcessor.SilenceDetected += OnSilenceDetected;
        _audioPreProcessor.NoiseDetected += OnNoiseDetected;
        _logs.Info("Audio pre-processor initialized");

        // Initialize speech recognizer if API key available
        if (!string.IsNullOrEmpty(_config.YandexApiKey))
        {
            _speechRecognizer = new YandexSpeechRecognizer(_logs, _config.YandexApiKey, "ru-RU");
            _speechRecognizer.TextRecognized += (s, text) => _ = OnTextRecognizedAsync(s, text);
            await _speechRecognizer.StartRecognitionAsync();
            _logs.Info("Speech recognizer initialized");
        }
        else
        {
            _logs.Warn("Yandex API key not found, speech recognition disabled");
        }

        // Initialize camera stream handler
        _cameraStream = new CameraStreamHandler(_logs, _config);
        _cameraStream.VideoFrameReceived += (s, data) => _ = OnVideoFrameReceivedAsync(s, data);
        _cameraStream.AudioFrameReceived += OnAudioFrameReceived;
        
        var cameraConnected = await _cameraStream.ConnectAsync();
        if (!cameraConnected)
        {
            _logs.Error("Failed to connect to camera");
            return;
        }
        _logs.Info("Camera connected");
    }

    public void Start()
    {
        _started = true;
        _ = _cameraStream?.StartStreamingAsync();
    }

    public async Task StopAsync()
    {
        _started = false;
        _cameraStream?.StopStreaming();
        
        // Flush any remaining audio in buffer
        _audioPreProcessor?.Flush();
        
        // Stop speech recognizer with timeout
        if (_speechRecognizer != null)
        {
            try
            {
                await _speechRecognizer.StopRecognitionAsync().WaitAsync(TimeSpan.FromSeconds(1));
            }
            catch (TimeoutException)
            {
                _logs.Warn("Timeout stopping speech recognizer");
            }
            catch (Exception ex)
            {
                _logs.Warn(ex, "Error stopping speech recognizer");
            }
        }
        
        // Dispose camera stream handler
        if (_cameraStream != null)
        {
            await _cameraStream.DisposeAsync();
        }
        
        _publisher?.Disconnect();
        
        // Log statistics (thread-safe reads)
        var total = Interlocked.Read(ref _totalAudioFrames);
        var filtered = Interlocked.Read(ref _filteredAudioFrames);
        var apiCalls = Interlocked.Read(ref _apiCalls);
        
        if (total > 0)
        {
            double filterRate = (filtered / (double)total) * 100.0;
            _logs.Info($"Audio processing stats: Total frames: {total}, Filtered: {filtered} ({filterRate:F1}%), API calls: {apiCalls}");
        }
    }

    private async Task OnVideoFrameReceivedAsync(object? sender, byte[] h264Frame)
    {
        if (!_started || _videoProcessor == null || _publisher == null) return;

        // Rate limiting: only process frames at configured FPS
        var now = DateTime.UtcNow;
        if (now - _lastVideoFrameTime < _videoFrameInterval)
        {
            return; // Skip this frame
        }
        _lastVideoFrameTime = now;

        try
        {
            var jpegData = await _videoProcessor.ProcessH264FrameAsync(h264Frame);
            if (jpegData != null && jpegData.Length > 0)
            {
                await _publisher.PublishVideoFrameAsync(jpegData);
            }
        }
        catch (Exception ex)
        {
            _logs.Error(ex, "Failed to process video frame");
        }
    }

    private void OnAudioFrameReceived(object? sender, byte[] pcmData)
    {
        if (!_started || _audioPreProcessor == null || pcmData.Length == 0) return;

        try
        {
            Interlocked.Increment(ref _totalAudioFrames);
            
            // Process through audio pre-processor (VAD, silence/noise filtering)
            _audioPreProcessor.ProcessAudioFrame(pcmData);
        }
        catch (Exception ex)
        {
            _logs.Warn(ex, "Failed to process audio frame");
        }
    }

    private async Task OnSpeechDetectedAsync(object? sender, byte[] audioData)
    {
        if (!_started || _speechRecognizer == null || audioData.Length == 0) return;

        try
        {
            // Check buffer size limit
            if (audioData.Length > _config.MaxAudioBufferSizeBytes)
            {
                _logs.Warn($"Audio buffer size limit exceeded ({audioData.Length} bytes), skipping");
                return;
            }

            Interlocked.Increment(ref _apiCalls);
            
            // Send batched audio to speech recognizer
            // Yandex streaming API handles large chunks internally
            await _speechRecognizer.AddAudioChunkAsync(audioData);
            
            _logs.Trace($"Sent {audioData.Length} bytes of speech audio to Yandex API");
        }
        catch (Exception ex)
        {
            _logs.Error(ex, "Failed to send speech audio to recognizer");
        }
    }

    private void OnSilenceDetected(object? sender, EventArgs e)
    {
        if (!_started) return;
        Interlocked.Increment(ref _filteredAudioFrames);
        _logs.Trace("Silence detected, filtered");
    }

    private void OnNoiseDetected(object? sender, EventArgs e)
    {
        if (!_started) return;
        Interlocked.Increment(ref _filteredAudioFrames);
        _logs.Trace("White noise detected, filtered");
    }

    private async Task OnTextRecognizedAsync(object? sender, string text)
    {
        if (!_started || _publisher == null || string.IsNullOrWhiteSpace(text)) return;

        try
        {
            await _publisher.PublishTextAsync(text);
            _logs.Info($"Published recognized text: {text}");
        }
        catch (Exception ex)
        {
            _logs.Error(ex, "Failed to publish recognized text");
        }
    }
}

