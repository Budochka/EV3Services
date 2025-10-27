using System;
using NLog;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;

namespace EV3UIWF
{
    class Worker
    { 
        private readonly Logger _logs;
        private RabbitConsumer _consumer;
        private RabbitPublisher _publisher;
        private readonly Config _config;

        private bool _started;

        //handling received messages
        public delegate void MessageReceivedHandler(string key, byte[] data);
        public event MessageReceivedHandler Notify;

        public Worker(Logger log, Config config)
        {
            _logs = log;
            _config = config;
            _started = false;
        }

        public async Task InitializeAsync()
        {
            _consumer = new RabbitConsumer(_logs);
            await _consumer.ConnectToRabbitAsync(_config.RabbitUserName,
                                      _config.RabbitPassword,
                                      _config.RabbitHost,
                                      _config.RabbitPort,
                                      HandleRabbitMessageAsync);
            _logs.Info("RabbitConsumer created");

            _publisher = new RabbitPublisher(_logs);
            await _publisher.ConnectToRabbitAsync(_config.RabbitUserName,
                                      _config.RabbitPassword,
                                      _config.RabbitHost,
                                      _config.RabbitPort);
            _logs.Info("RabbitPublisher created");
        }

        public void Start()
        {
            _started = true;
        }

        public void Stop()
        {
            _started = false;
        }

        public async Task PublishAsync(string key, byte[] data)
        {
            await _publisher.PublishAsync(key, data);
        }

        private async Task HandleRabbitMessageAsync(object sender, BasicDeliverEventArgs args)
        {
            var bytes = args.Body.ToArray();
            if ((bytes.Length > 0) && _started)
            {
                var buffer = new byte[bytes.Length / sizeof(byte)];
                Buffer.BlockCopy(bytes, 0, buffer, 0, bytes.Length);
                Notify?.Invoke(args.RoutingKey, buffer);
            }

            var ec = (AsyncEventingBasicConsumer)sender;
            await ec.Channel.BasicAckAsync(args.DeliveryTag, false);
        }
    }
}
