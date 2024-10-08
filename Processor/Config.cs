namespace Processor
{
    class Config
    {
        public string? LogFileName { get; init; }
        public string? RabbitUserName { get; init; }
        public string? RabbitPassword { get; init; }
        public string? RabbitHost { get; init; }
        public int RabbitPort { get; init; }
    }
}
