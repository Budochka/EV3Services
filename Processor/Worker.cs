using NLog;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;
using EV3Services.Common;

namespace Processor
{
    class Worker
    { 
        private readonly Logger _logs;
        private RabbitMQConsumer? _consumer;
        private RabbitMQPublisher? _publisher;
        private readonly Config _config;

        private RobotStateMachine _robotStateMachine;

        public Worker(Logger log, Config config)
        {
            _logs = log;
            _config = config;
            _robotStateMachine = new RobotStateMachine(_logs);
        }

        public async Task InitializeAsync()
        {
            //Create RabbitMQ consumer
            _consumer = new RabbitMQConsumer(_logs);
            await _consumer.ConnectToRabbitAsync(
                _config.RabbitUserName,
                _config.RabbitPassword,
                _config.RabbitHost,
                _config.RabbitPort,
                HandleRabbitMessageAsync,
                routingKeys: new[] { "sensors.*", "state.*" },
                autoAck: false);
            _logs.Info("RabbitConsumer created");

            //Create RabbitMQ producer
            _publisher = new RabbitMQPublisher(_logs);
            await _publisher.ConnectToRabbitAsync(
                _config.RabbitUserName,
                _config.RabbitPassword,
                _config.RabbitHost,
                _config.RabbitPort);
            _logs.Info("RabbitPublisher created");
        }

        public void Start()
        {
            // No-op: kept for API compatibility
        }

        public void Stop()
        {
            // No-op: kept for API compatibility
        }

        private async Task HandleRabbitMessageAsync(object sender, BasicDeliverEventArgs args)
        {
            var key = args.RoutingKey;
            var consumer = sender as AsyncEventingBasicConsumer;
            bool handled = false;

            try
            {
                // State handler
                if (key == "state.greet" || key == "state.direct" || key == "state.explore")
                {
                    _robotStateMachine.SetState(key);
                    handled = true;
                }
                // Touch handler
                else if (key == "sensors.touch")
                {
                    if (_publisher != null)
                    {
                        await _publisher.PublishAsync("voice.text", Encoding.Unicode.GetBytes("Привет! Держи пять!"));
                    }
                    handled = true;
                }
                // Distance handler
                else if (key == "sensors.distance")
                {
                    float distance = System.BitConverter.ToSingle(args.Body.ToArray());
                    _robotStateMachine.WorldModel.Distance = distance;
                    handled = true;
                }
                // Faces handler
                else if (key == "faces.ids")
                {
                    // TODO: Process face IDs when needed
                    handled = true;
                }

                // Acknowledge message after processing
                if (handled && consumer != null)
                {
                    await consumer.Channel.BasicAckAsync(args.DeliveryTag, false);
                }
                else if (consumer != null)
                {
                    // Reject unknown messages
                    await consumer.Channel.BasicNackAsync(args.DeliveryTag, false, false);
                    _logs.Warn("Unknown routing key: {0}", key);
                }
            }
            catch (Exception ex)
            {
                _logs.Error(ex, "Error processing message with routing key: {0}", key);
                // Reject and requeue on error
                if (consumer != null)
                {
                    await consumer.Channel.BasicNackAsync(args.DeliveryTag, false, true);
                }
            }
        }
    }
}
