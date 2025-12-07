using NLog;

namespace VideoAudioProcessor;

/// <summary>
/// Audio pre-processor that filters silence, white noise, and implements Voice Activity Detection (VAD)
/// to minimize API calls to Yandex Speech Recognition.
/// </summary>
public class AudioPreProcessor
{
    private readonly Config _config;
    private readonly Logger _logs;
    private readonly AudioBuffer _audioBuffer;
    private readonly AdaptiveNoiseFloor? _adaptiveNoiseFloor;

    // State tracking
    private enum AudioState
    {
        Silence,           // No speech detected
        SpeechBuffering,   // Collecting speech audio
        Noise              // White noise detected, skip
    }
    
    // Constants for noise detection thresholds
    private const double VarianceMultiplier = 10.0;      // Speech has much higher variance than silence
    private const double NoiseZCRThreshold = 0.8;        // ZCR threshold for noise detection (80% of max)
    private const double NoiseZCRPattern = 0.7;        // ZCR pattern threshold for noise (70% of max)
    private const int MinSampleLengthForAutocorrelation = 20; // Minimum bytes for reliable autocorrelation

    private AudioState _currentState = AudioState.Silence;
    private int _consecutiveSilentFrames = 0;

    // Events
    public event EventHandler<byte[]>? SpeechDetected;  // Send to Yandex
    public event EventHandler? SilenceDetected;         // Skip processing
    public event EventHandler? NoiseDetected;            // Skip processing

    public AudioPreProcessor(Logger logs, Config config)
    {
        _logs = logs;
        _config = config;
        _audioBuffer = new AudioBuffer(config.MinBufferDurationMs, config.MaxBufferDurationMs);

        if (config.EnableAdaptiveThreshold)
        {
            _adaptiveNoiseFloor = new AdaptiveNoiseFloor(
                config.AdaptationWindowSize,
                config.NoiseFloor);
        }
    }

    /// <summary>
    /// Process an audio frame and determine if it should be sent to API.
    /// </summary>
    public void ProcessAudioFrame(byte[] pcmData)
    {
        if (pcmData == null || pcmData.Length == 0)
        {
            return;
        }

        // Calculate audio metrics
        double rms = CalculateRMS(pcmData);
        double zcr = CalculateZCR(pcmData);

        // Update adaptive noise floor if enabled
        if (_adaptiveNoiseFloor != null)
        {
            _adaptiveNoiseFloor.Update(rms);
        }

        // Process based on current state
        ProcessFrameByState(pcmData, rms, zcr);
    }

    private void ProcessFrameByState(byte[] pcmData, double rms, double zcr)
    {
        switch (_currentState)
        {
            case AudioState.Silence:
                if (IsSilence(pcmData, rms, zcr))
                {
                    _consecutiveSilentFrames++;
                    SilenceDetected?.Invoke(this, EventArgs.Empty);
                }
                else if (IsWhiteNoise(pcmData, rms, zcr))
                {
                    _currentState = AudioState.Noise;
                    NoiseDetected?.Invoke(this, EventArgs.Empty);
                    _logs.Trace("White noise detected, skipping");
                }
                else if (DetectVoiceActivity(pcmData, rms, zcr))
                {
                    _currentState = AudioState.SpeechBuffering;
                    _audioBuffer.Clear();
                    _audioBuffer.AddFrame(pcmData);
                    _consecutiveSilentFrames = 0;
                    _logs.Debug("Speech detected, starting buffer");
                }
                break;

            case AudioState.SpeechBuffering:
                if (IsSilence(pcmData, rms, zcr))
                {
                    _consecutiveSilentFrames++;
                    _audioBuffer.MarkSilence();

                    if (_consecutiveSilentFrames > _config.SilenceFramesToEnd)
                    {
                        FlushBufferToYandex();
                        _currentState = AudioState.Silence;
                        _consecutiveSilentFrames = 0;
                        _logs.Debug("Speech ended (silence), flushed buffer");
                    }
                }
                else if (IsWhiteNoise(pcmData, rms, zcr))
                {
                    _currentState = AudioState.Noise;
                    _audioBuffer.Clear();
                    _consecutiveSilentFrames = 0;
                    NoiseDetected?.Invoke(this, EventArgs.Empty);
                    _logs.Debug("White noise detected during speech, clearing buffer");
                }
                else if (DetectVoiceActivity(pcmData, rms, zcr))
                {
                    _audioBuffer.AddFrame(pcmData);
                    _consecutiveSilentFrames = 0;

                    // Check if buffer should be flushed (max duration reached)
                    if (_audioBuffer.ShouldFlush())
                    {
                        FlushBufferToYandex();
                        // Continue buffering if still in speech
                        _audioBuffer.Clear();
                        _audioBuffer.AddFrame(pcmData);
                    }
                }
                break;

            case AudioState.Noise:
                if (IsSilence(pcmData, rms, zcr))
                {
                    _currentState = AudioState.Silence;
                    _consecutiveSilentFrames++;
                }
                else if (DetectVoiceActivity(pcmData, rms, zcr) && !IsWhiteNoise(pcmData, rms, zcr))
                {
                    _currentState = AudioState.SpeechBuffering;
                    _audioBuffer.Clear();
                    _audioBuffer.AddFrame(pcmData);
                    _consecutiveSilentFrames = 0;
                    _logs.Debug("Speech detected after noise, starting buffer");
                }
                break;
        }

        // Safety: Reset if too many silent frames
        if (_consecutiveSilentFrames > _config.MaxSilentFrames)
        {
            _currentState = AudioState.Silence;
            _audioBuffer.Clear();
            _consecutiveSilentFrames = 0;
        }
    }

    private void FlushBufferToYandex()
    {
        var audioData = _audioBuffer.Flush();
        if (audioData.Length > 0)
        {
            SpeechDetected?.Invoke(this, audioData);
            _logs.Debug($"Flushed {audioData.Length} bytes of audio to Yandex API");
        }
    }

    /// <summary>
    /// Calculate RMS (Root Mean Square) energy of audio signal.
    /// </summary>
    private double CalculateRMS(byte[] pcmData)
    {
        if (pcmData.Length < 2) return 0.0;

        long sumSquares = 0;
        int sampleCount = pcmData.Length / 2; // 16-bit samples

        for (int i = 0; i < pcmData.Length - 1; i += 2)
        {
            short sample = (short)(pcmData[i] | (pcmData[i + 1] << 8));
            sumSquares += (long)sample * sample;
        }

        if (sampleCount == 0) return 0.0;
        return Math.Sqrt(sumSquares / (double)sampleCount);
    }

    /// <summary>
    /// Calculate Zero-Crossing Rate (ZCR) of audio signal.
    /// </summary>
    private double CalculateZCR(byte[] pcmData)
    {
        if (pcmData.Length < 4) return 0.0;

        int zeroCrossings = 0;
        short previousSample = 0;
        int sampleCount = 0;

        for (int i = 0; i < pcmData.Length - 1; i += 2)
        {
            short sample = (short)(pcmData[i] | (pcmData[i + 1] << 8));

            // Detect zero crossing
            if ((previousSample >= 0 && sample < 0) ||
                (previousSample < 0 && sample >= 0))
            {
                zeroCrossings++;
            }

            previousSample = sample;
            sampleCount++;
        }

        if (sampleCount == 0) return 0.0;
        // ZCR as ratio (0.0 to 1.0) for 8kHz sample rate
        return zeroCrossings / (double)sampleCount;
    }

    /// <summary>
    /// Calculate variance of audio samples (silence has very low variance).
    /// </summary>
    private double CalculateVariance(byte[] pcmData)
    {
        if (pcmData.Length < 4) return 0.0;

        int sampleCount = pcmData.Length / 2;
        long sum = 0;
        long sumSquares = 0;

        for (int i = 0; i < pcmData.Length - 1; i += 2)
        {
            short sample = (short)(pcmData[i] | (pcmData[i + 1] << 8));
            sum += sample;
            sumSquares += (long)sample * sample;
        }

        if (sampleCount == 0) return 0.0;
        double mean = sum / (double)sampleCount;
        double variance = (sumSquares / (double)sampleCount) - (mean * mean);
        return variance;
    }

    /// <summary>
    /// Detect if audio contains voice activity.
    /// </summary>
    private bool DetectVoiceActivity(byte[] pcmData, double rms, double zcr)
    {
        // Method 1: Energy threshold (adaptive if enabled)
        double threshold;
        if (_adaptiveNoiseFloor != null)
        {
            threshold = _adaptiveNoiseFloor.GetThreshold(_config.EnergyMultiplier);
        }
        else
        {
            threshold = _config.EnergyThreshold;
        }

        bool hasEnergy = rms > threshold;

        // Method 2: Zero-crossing rate (speech has moderate ZCR)
        bool hasValidZCR = zcr > _config.MinZCR && zcr < _config.MaxZCR;

        // Method 3: Variance (speech has higher variance than silence)
        double variance = CalculateVariance(pcmData);
        bool hasVariance = variance > _config.SilenceThreshold * VarianceMultiplier;

        // Speech must pass all three checks
        return hasEnergy && hasValidZCR && hasVariance;
    }

    /// <summary>
    /// Detect if audio is silence.
    /// </summary>
    private bool IsSilence(byte[] pcmData, double rms, double zcr)
    {
        // Method 1: Energy threshold
        if (rms < _config.SilenceThreshold)
        {
            return true;
        }

        // Method 2: Zero-crossing rate (silence has very low ZCR)
        if (zcr < _config.MinZCR)
        {
            return true;
        }

        // Method 3: Sample variance (silence has very low variance)
        double variance = CalculateVariance(pcmData);
        if (variance < _config.SilenceThreshold)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Detect if audio is white noise.
    /// </summary>
    private bool IsWhiteNoise(byte[] pcmData, double rms, double zcr)
    {
        // White noise characteristics:
        // 1. High zero-crossing rate
        if (zcr > _config.MaxZCR)
        {
            return true;
        }

        // Method 2: Check spectral characteristics (simplified)
        // White noise has energy spread across frequencies
        // We use autocorrelation as a proxy (noise has low autocorrelation)
        // Skip autocorrelation for very short samples (not enough data for reliable calculation)
        if (pcmData.Length >= MinSampleLengthForAutocorrelation)
        {
            double autocorrelation = CalculateAutocorrelation(pcmData);
            if (autocorrelation < _config.SpeechAutocorrelationThreshold)
            {
                // Also check if ZCR is high (noise indicator)
                if (zcr > _config.MaxZCR * NoiseZCRThreshold)
                {
                    return true;
                }
            }
        }

        // Method 3: High energy with high ZCR (noise pattern)
        if (rms > _config.EnergyThreshold && zcr > _config.MaxZCR * NoiseZCRPattern)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Calculate autocorrelation of audio signal (speech has structure, noise doesn't).
    /// Simplified version using sample correlation.
    /// Caller should ensure pcmData.Length >= 20 before calling this method.
    /// </summary>
    private double CalculateAutocorrelation(byte[] pcmData)
    {
        // Calculate correlation between adjacent samples
        // Speech has higher correlation than noise
        int correlations = 0;
        int totalPairs = 0;

        for (int i = 0; i < pcmData.Length - 4; i += 2)
        {
            short sample1 = (short)(pcmData[i] | (pcmData[i + 1] << 8));
            short sample2 = (short)(pcmData[i + 2] | (pcmData[i + 3] << 8));

            // Check if samples have similar sign and magnitude (correlation indicator)
            if (Math.Sign(sample1) == Math.Sign(sample2))
            {
                double ratio = Math.Abs(sample1) > 0 
                    ? Math.Abs(sample2) / (double)Math.Abs(sample1) 
                    : 0;
                if (ratio > 0.5 && ratio < 2.0) // Similar magnitude
                {
                    correlations++;
                }
            }
            totalPairs++;
        }

        if (totalPairs == 0) return 0.0;
        return correlations / (double)totalPairs;
    }

    /// <summary>
    /// Force flush any buffered audio (e.g., on shutdown).
    /// </summary>
    public void Flush()
    {
        if (_currentState == AudioState.SpeechBuffering)
        {
            FlushBufferToYandex();
        }
        _currentState = AudioState.Silence;
        _audioBuffer.Clear();
    }

    /// <summary>
    /// Get current statistics for monitoring.
    /// </summary>
    public (string State, int BufferFrames, int BufferBytes, double NoiseFloor) GetStats()
    {
        return (
            _currentState.ToString(),
            _audioBuffer.FrameCount,
            _audioBuffer.TotalBytes,
            _adaptiveNoiseFloor?.CurrentNoiseFloor ?? _config.NoiseFloor
        );
    }
}

