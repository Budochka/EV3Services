using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace VoiceCreator
{
    class RabbitConnector
    {
        private IModel _channel;
        private IConnection _connection;
        private Logger _logs;

        public RabbitConnector(Logger log)
        {
            _logs = log;
        }

        public bool ConnectToRabbit(in string user, in string pass, in string vhost, in string hostName, in int port, in EventHandler<BasicDeliverEventArgs> ev)
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = user,
                Password = pass,
                VirtualHost = vhost,
                HostName = hostName,
                Port = port,
                AutomaticRecoveryEnabled = true
            };

            //creating connection
            _logs.Info("Creating Rabbit MQ connection host:{0}, port: {1}", hostName, port);
            _connection = factory.CreateConnection();
            if (_connection != null)
            {
                _logs.Info("Connection created");
                _channel = _connection.CreateModel();
                if (_channel != null)
                {
                    _logs.Info("Channel created");
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            //declaring exchange 
            _channel.ExchangeDeclare(exchange: "EV3", type: "topic", autoDelete: true);
            _logs.Info("Exchange created");

            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: queueName, exchange: "EV3", routingKey: "voice.text");
            _logs.Info("Queue binding complete");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += ev;
            _logs.Info("Event hadler set");

            return true;
        }

        ~RabbitConnector()
        {
            if (_channel != null)
            {
                _channel.Close();
            }

            if (_connection != null)
            {
                _connection.Close();
            }
        }
    }
}
