using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace VoiceCreator
{
    class RabbitPublisher
    {
        private IModel _channel;
        private IConnection _connection;
        private Logger _logs;

        public RabbitPublisher(Logger log)
        {
            _logs = log;
        }

        public bool ConnectToRabbit(in string user, in string pass, in string hostName, in int port)
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = user,
                Password = pass,
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

            return true;
        }

        ~RabbitPublisher()
        {
            _logs.Info("Destructor called");

            _channel?.Close();

            _connection?.Close();
        }

        public void Publish(in byte[] data)
        {
            _channel?.BasicPublish(exchange: "EV3", routingKey: "voice.generated.wav", basicProperties: null, body: data);
        }
    }
}