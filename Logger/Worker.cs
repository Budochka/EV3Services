using System;
using System.IO;
using NLog;
using RabbitMQ.Client.Events;
using MySqlConnector;
using System.Data;
using System.Reflection.Metadata;

namespace Logger
{
    class Worker
    { 
        private readonly NLog.Logger _logs;
        private RabbitConsumer? _consumer;
        private readonly Config _config;
        private MySqlConnection? _connection;

        private bool _started;

        public Worker(NLog.Logger log, Config config)
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

            _connection = new MySqlConnection(_config.ConnectionString);
            _connection.Open();
            _logs.Info("Database connection opened");
        }

        public void Start()
        {
            _started = true;
        }

        public void Stop()
        {
            _started = false;
        }

        private void HandleRabbitMessage(object? sender, BasicDeliverEventArgs args)
        {
            var bytes = args.Body.ToArray();
            if ((bytes.Length > 0) && _started && (_connection != null))
            {
                using var command = _connection.CreateCommand();
                command.CommandText = @"insert into Events (Time, Topic, Data) values (@time, @topic, @data);";
                command.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@time",
                    DbType = DbType.DateTime,
                    Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff")
                });
                command.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@topic",
                    DbType = DbType.String,
                    Value = args.RoutingKey
                });
                command.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@data",
                    DbType = DbType.Binary,
                    Value = bytes
                });
                command.ExecuteNonQueryAsync();
            }
        }
    }
}
