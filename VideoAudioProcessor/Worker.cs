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
    
    private bool _started = false;
    private DateTime _lastVideoFrameTime = DateTime.MinValue;
    private readonly TimeSpan _videoFrameInterval;
    
    // Audio buffering for speech recognition (Yandex streaming handles silence, but we track for buffer limits)
    private long _audioBufferSize = 0;

    public Worker(Logger log, Config? config)
    {
        _logs = log;
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _videoFrameInterval = TimeSpan.FromSeconds(1.0 / _config.VideoFps);
    }

    public async Task InitializeAsync()
    {
        if (_config == null) return;

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

        // Initialize speech recognizer if API key available
        if (!string.IsNullOrEmpty(_config.YandexApiKey))
        {
            _speechRecognizer = new YandexSpeechRecognizer(_logs, _config.YandexApiKey, "ru-RU");
            _speechRecognizer.TextRecognized += OnTextRecognized;
            await _speechRecognizer.StartRecognitionAsync();
            _logs.Info("Speech recognizer initialized");
        }
        else
        {
            _logs.Warn("Yandex API key not found, speech recognition disabled");
        }

        // Initialize camera stream handler
        _cameraStream = new CameraStreamHandler(_logs, _config);
        _cameraStream.VideoFrameReceived += OnVideoFrameReceived;
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
        if (_cameraStream != null)
        {
            _ = _cameraStream.StartStreamingAsync();
        }
    }

    public void Stop()
    {
        _started = false;
        _cameraStream?.StopStreaming();
        _speechRecognizer?.StopRecognitionAsync().Wait(1000);
        _publisher?.Disconnect();
    }

    private async void OnVideoFrameReceived(object? sender, byte[] h264Frame)
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
            _logs.Warn(ex, "Failed to process video frame");
        }
    }

    private async void OnAudioFrameReceived(object? sender, byte[] pcmData)
    {
        if (!_started || _speechRecognizer == null || pcmData.Length == 0) return;

        try
        {
            // Check buffer size limit (simple counter, Yandex streaming handles actual buffering)
            if (_audioBufferSize + pcmData.Length > _config.MaxAudioBufferSizeBytes)
            {
                _logs.Warn("Audio buffer size limit reached, skipping chunk");
                return;
            }

            _audioBufferSize += pcmData.Length;

            // Send to speech recognizer (streaming mode handles silence detection automatically)
            await _speechRecognizer.AddAudioChunkAsync(pcmData);
        }
        catch (Exception ex)
        {
            _logs.Warn(ex, "Failed to process audio frame");
        }
    }

    private async void OnTextRecognized(object? sender, string text)
    {
        if (!_started || _publisher == null || string.IsNullOrWhiteSpace(text)) return;

        try
        {
            await _publisher.PublishTextAsync(text);
            _logs.Info($"Published recognized text: {text}");
        }
        catch (Exception ex)
        {
            _logs.Warn(ex, "Failed to publish recognized text");
        }
    }
}

