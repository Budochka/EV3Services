using System;
using System.Globalization;
using System.IO;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EV3UI
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

        public void Initialize()
        {
            _consumer = new RabbitConsumer(_logs);
            _consumer.ConnectToRabbit(_config.RabbitUserName,
                                      _config.RabbitPassword,
                                      _config.RabbitHost,
                                      _config.RabbitPort,
                                      HandleRabbitMessage);
            _logs.Info("RabbitConsumer created");

            _publisher = new RabbitPublisher(_logs);
            _publisher.ConnectToRabbit(_config.RabbitUserName,
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

        public void Publish(string key, in byte[] data)
        {
            _publisher.Publish(key, data);
        }

        private void HandleRabbitMessage(object sender, BasicDeliverEventArgs args)
        {
            var bytes = args.Body.ToArray();
            if ((bytes.Length > 0) && _started)
            {
                var buffer = new byte[bytes.Length / sizeof(byte)];
                Buffer.BlockCopy(bytes, 0, buffer, 0, bytes.Length);
                Notify?.Invoke(args.RoutingKey, buffer);
            }

            var ec = (EventingBasicConsumer)sender;
            ec.Model.BasicAck(args.DeliveryTag, false);
        }
    }
}
