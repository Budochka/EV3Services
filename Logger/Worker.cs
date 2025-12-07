using RabbitMQ.Client.Events;
using Npgsql;
using System.Linq;
using System.Threading.Tasks;
using EV3Services.Common;

namespace Logger
{
    class Worker
    { 
        private readonly NLog.Logger _logs;
        private RabbitMQConsumer? _consumer;
        private readonly Config _config;
        private NpgsqlConnection? _connection;

        private bool _started;

        public Worker(NLog.Logger log, Config config)
        {
            _logs = log;
            _config = config;
            _started = false;
        }

        public async Task InitializeAsync()
        {
            _consumer = new RabbitMQConsumer(_logs);
            await _consumer.ConnectToRabbitAsync(
                _config.RabbitUserName,
                _config.RabbitPassword,
                _config.RabbitHost,
                _config.RabbitPort,
                HandleRabbitMessageAsync,
                traceExchange: true,
                autoAck: false);
            _logs.Info("RabbitConsumer created");

            _connection = new NpgsqlConnection(_config.ConnectionString);
            await _connection.OpenAsync();
            _logs.Info("PostgreSQL connection opened");
        }

        public void Start()
        {
            _started = true;
        }

        public void Stop()
        {
            _started = false;
        }

        private async Task HandleRabbitMessageAsync(object? sender, BasicDeliverEventArgs args)
        {
            var bytes = args.Body.ToArray();
            var consumer = sender as AsyncEventingBasicConsumer;
            
            if ((bytes.Length > 0) && _started && (_connection != null) && consumer != null)
            {
                try
                {
                    // Parse trace routing key to extract original routing key
                    // Trace format: "publish.exchange.routing_key" or "deliver.queue.routing_key"
                    string originalRoutingKey = ExtractOriginalRoutingKey(args.RoutingKey);
                    
                    await using var command = new NpgsqlCommand(
                        @"INSERT INTO Events (Time, Topic, Data) VALUES (@time, @topic, @data);",
                        _connection);
                    
                    command.Parameters.AddWithValue("@time", DateTime.UtcNow);
                    // Store original routing key (not the trace routing key)
                    command.Parameters.AddWithValue("@topic", originalRoutingKey);
                    command.Parameters.AddWithValue("@data", bytes.Length > 0 ? (object)bytes : DBNull.Value);
                    
                    await command.ExecuteNonQueryAsync();
                    
                    // Acknowledge message only after successful logging
                    await consumer.Channel.BasicAckAsync(args.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logs.Error(ex, "Error logging message to database. Message will be requeued. Trace routing key: {0}", args.RoutingKey);
                    // Reject and requeue on error
                    await consumer.Channel.BasicNackAsync(args.DeliveryTag, false, true);
                }
            }
            else if (consumer != null)
            {
                // If not started or no connection, acknowledge to prevent message buildup
                await consumer.Channel.BasicAckAsync(args.DeliveryTag, false);
            }
        }

        /// <summary>
        /// Extracts the original routing key from a trace routing key.
        /// Trace format: "publish.exchange.routing_key" or "deliver.queue.routing_key"
        /// Returns the original routing key (everything after the second dot).
        /// </summary>
        private string ExtractOriginalRoutingKey(string traceRoutingKey)
        {
            if (string.IsNullOrEmpty(traceRoutingKey))
                return traceRoutingKey;

            // Trace routing keys have format: "publish.exchange.routing_key" or "deliver.queue.routing_key"
            // We need to extract the original routing key (everything after the second dot)
            var parts = traceRoutingKey.Split('.');
            
            if (parts.Length >= 3)
            {
                // Skip "publish"/"deliver" and exchange/queue name, return the rest
                return string.Join(".", parts.Skip(2));
            }
            
            // If format is unexpected, return as-is (shouldn't happen with proper trace)
            _logs.Warn("Unexpected trace routing key format: {0}", traceRoutingKey);
            return traceRoutingKey;
        }
    }
}
