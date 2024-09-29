using NLog;
using RabbitMQ.Client.Events;

namespace Processor
{
    class Worker
    { 
        private readonly Logger _logs;
        private RabbitConsumer? _consumer;
        private RabbitPublisher? _publisher;
        private readonly Config _config;

        private RobotStateMachine _robotStateMachine;

        private List<IMessageHandler> _handlers;

        private bool _started;

        public Worker(Logger log, Config config)
        {
            _logs = log;
            _config = config;
            _started = false;
            _handlers = [];
        }

        public void Initialize()
        {
            //Create RabitMQ consumer
            _consumer = new RabbitConsumer(_logs);
            _consumer.ConnectToRabbit(_config.RabbitUserName,
                                      _config.RabbitPassword,
                                      _config.RabbitHost,
                                      _config.RabbitPort,
                                      HandleRabbitMessage);
            _logs.Info("RabbitConsumer created");

            //Create RabitMQ producer
            _publisher = new RabbitPublisher(_logs);
            _publisher.ConnectToRabbit(_config.RabbitUserName,
                                      _config.RabbitPassword,
                                      _config.RabbitHost,
                                      _config.RabbitPort);
            _logs.Info("RabbitPublisher created");

            //Create world model
            _robotStateMachine = new RobotStateMachine();

            //Initialize list of message handlers
            _handlers.Add(new TouchHandler());
            _handlers.Add(new DistanceHandler());
            _handlers.Add(new FacesHanler());
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
            foreach (var messageHandler in _handlers.TakeWhile(messageHandler => _publisher == null || !messageHandler.HandleRabbitMessage(_robotStateMachine, _publisher, sender, args)))
            {
            }
        }
    }
}
