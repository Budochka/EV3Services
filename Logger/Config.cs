namespace Logger
{
    class Config
    {
        public string? LogFileName => null;
        public string? RabbitUserName => null;
        public string? RabbitPassword => null;

        public string? RabbitVHost => null;
        public string? RabbitHost => null;

        public int RabbitPort { get; set; }
        public string? ConnectionString => null;
    }
}
