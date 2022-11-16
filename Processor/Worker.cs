using NLog;
using RabbitMQ.Client.Events;
using System.Text;

namespace Processor
{
    class Worker
    { 
        private readonly Logger _logs;
        private RabbitConsumer _consumer;
        private RabbitPublisher _publisher;
        private readonly Config _config;

        private bool _started;

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

        private void HandleRabbitMessage(object sender, BasicDeliverEventArgs args)
        {
            if (args.RoutingKey == "sensors.touch")
            {
                _publisher.Publish("voice.text", Encoding.Unicode.GetBytes("Привет! Держи пять!"));
                EventingBasicConsumer ec = (EventingBasicConsumer)sender;
                ec.Model.BasicAck(args.DeliveryTag, false);
            }
        }
    }
}
