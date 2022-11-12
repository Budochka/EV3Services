namespace Logger
{
    class Config
    {
        public string LogFileName { get; set; }
        public string RabbitUserName { get; set; }
        public string RabbitPassword { get; set; }
        public string RabbitVHost { get; set; }
        public string RabbitHost { get; set; }
        public int RabbitPort { get; set; }
        public string ConnectionString { get; set; }
    }
}
