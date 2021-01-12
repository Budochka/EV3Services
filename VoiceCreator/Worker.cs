using System;
using System.IO;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace VoiceCreator
{
    class Worker
    { 
        private Logger _logs;
        private RabbitConsumer _consumer;
        private RabbitPublisher _publisher;
        private VoiceSynthesis _vs;
        private Config _config;

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

            _vs = new VoiceSynthesis(_logs);
            _logs.Info("VoiceSyntheses created");
        }

        public void Start()
        {
            _started = true;
        }

        public void Stop()
        {
            _started = false;
        }

        public void HandleRabbitMessage(object sender, BasicDeliverEventArgs args)
        {
            string body = args.Body.ToString();
            if ((body.Length > 0) && _started)
            {
                string file;

                if (_vs != null)
                {
                    _vs.Text2File(body, out file);
                    var filedata = File.ReadAllBytes(file);
                    File.Delete(file);
                    _publisher.Publish(filedata);
                }
            }

            ((IModel)sender).BasicAck(args.DeliveryTag, false);
        }
    }
}
