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
    public int SilenceDurationMs { get; init; } = 3000;
    
    // Yandex API key (loaded from separate file)
    public string? YandexApiKey { get; init; }
}

