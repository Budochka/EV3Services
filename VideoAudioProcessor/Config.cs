namespace VideoAudioProcessor;

public record Config
{
    public string? LogFileName { get; init; }
    
    // RabbitMQ settings
    public string? RabbitUserName { get; init; }
    public string? RabbitPassword { get; init; }
    public string? RabbitHost { get; init; }
    public int RabbitPort { get; init; }
    
    // Camera settings (from v380stream)
    public string? CameraIP { get; init; }
    public int CameraPort { get; init; }
    public string? CameraID { get; init; }
    public string? CameraUserName { get; init; }
    public string? CameraPassword { get; init; }
    
    // Video processing settings
    public double VideoFps { get; init; } = 10.0;
    public int VideoWidth { get; init; } = 640;
    public int JpegQuality { get; init; } = 85;
    
    // Audio processing settings
    public int MaxAudioBufferSizeBytes { get; init; } = 10 * 1024 * 1024;
    
    // VAD Settings
    public double EnergyThreshold { get; init; } = 500.0;        // RMS threshold
    public double EnergyMultiplier { get; init; } = 2.0;         // Multiplier for adaptive threshold
    public double SilenceThreshold { get; init; } = 100.0;       // RMS for silence
    public double NoiseFloor { get; init; } = 200.0;            // Baseline noise level
    
    // Zero-Crossing Rate
    public double MinZCR { get; init; } = 0.01;                 // Min ZCR for speech (100 crossings/sec at 8kHz)
    public double MaxZCR { get; init; } = 0.375;                // Max ZCR for speech (3000 crossings/sec)
    
    // Spectral Analysis
    public double SpeechAutocorrelationThreshold { get; init; } = 0.3; // Autocorrelation for speech
    
    // Buffering
    public int MinBufferDurationMs { get; init; } = 500;        // Min audio before sending
    public int MaxBufferDurationMs { get; init; } = 3000;       // Max buffer before forced flush
    public int SilenceFramesToEnd { get; init; } = 15;           // Frames of silence to end speech
    public int MaxSilentFrames { get; init; } = 30;             // Max silent frames before reset
    
    // Adaptive Learning
    public bool EnableAdaptiveThreshold { get; init; } = true;  // Learn noise floor
    public int AdaptationWindowSize { get; init; } = 100;       // Frames for adaptation
    
    // Yandex API key (loaded from separate file)
    public string? YandexApiKey { get; init; }
}

