using System;
using RabbitMQ.Client;
using NLog;

namespace VoiceCreator
{
    class RabbitConnector
    {
        private IConnection _connection;
        private Logger _logs;

        public RabbitConnector(Logger log)
        {
            _logs = log;
        }

        private bool ConnectToRabbit(in string user, in string pass, in string vhost, in string hostName, in int port)
        {
            ConnectionFactory factory = new ConnectionFactory();
            // "guest"/"guest" by default, limited to localhost connections
            factory.UserName = user;
            factory.Password = pass;
            factory.VirtualHost = vhost;
            factory.HostName = hostName;
            factory.Port = port;

            _logs.Info("Creating Rabbit MQ connection host:{0}, port: {1}", hostName, port);
            _connection = factory.CreateConnection();
            return (_connection != null);
        }

        ~RabbitConnector()
        {
            _connection.Close();
        }
    }
}
