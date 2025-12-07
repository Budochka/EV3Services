using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;

namespace Logger
{
    class RabbitConsumer : IAsyncDisposable
    {
        private IChannel? _channel;
        private IConnection? _connection;
        private NLog.Logger _logs;

        public RabbitConsumer(NLog.Logger log)
        {
            _logs = log;
        }

        public async Task<bool> ConnectToRabbitAsync(string user, string pass, string hostName, int port, AsyncEventHandler<BasicDeliverEventArgs> ev)
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
            try
            {
                _connection = await factory.CreateConnectionAsync();
            }
            catch (Exception ex)
            {
                _logs.Error(ex, "Error creating RabbitMQ connection");
                return false;
            }

            if (_connection != null)
            {
                _logs.Info("Connection created");
                _channel = await _connection.CreateChannelAsync();
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

            // Connect to RabbitMQ Firehose Tracer exchange
            // amq.rabbitmq.trace is a built-in internal exchange that receives copies of all messages
            // when tracing is enabled (rabbitmqctl trace_on)
            // Trace routing keys: "publish.exchange.routing_key" or "deliver.queue.routing_key"
            var queueDeclareResult = await _channel.QueueDeclareAsync();
            var queueName = queueDeclareResult.QueueName;
            
            // Bind to trace exchange - this exchange already exists (internal, durable)
            // We don't need to declare it, just bind to it
            await _channel.QueueBindAsync(queue: queueName, exchange: "amq.rabbitmq.trace", routingKey: "#");
            _logs.Info("Queue bound to amq.rabbitmq.trace exchange (Firehose Tracer)");

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += ev;
            await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);

            _logs.Info("Event handler set");

            return true;
        }

        public async ValueTask DisposeAsync()
        {
            _logs.Info("DisposeAsync called");

            if (_channel != null)
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
            }

            if (_connection != null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
        }
    }
}
