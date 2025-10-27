using NLog;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;

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

        public async Task InitializeAsync()
        {
            //Create RabitMQ consumer
          _consumer = new RabbitConsumer(_logs);
     await _consumer.ConnectToRabbitAsync(_config.RabbitUserName,
         _config.RabbitPassword,
      _config.RabbitHost,
                _config.RabbitPort,
                 HandleRabbitMessageAsync);
 _logs.Info("RabbitConsumer created");

            //Create RabitMQ producer
            _publisher = new RabbitPublisher(_logs);
    await _publisher.ConnectToRabbitAsync(_config.RabbitUserName,
         _config.RabbitPassword,
            _config.RabbitHost,
           _config.RabbitPort);
     _logs.Info("RabbitPublisher created");

     //Create world model
     _robotStateMachine = new RobotStateMachine(_logs);

            //Initialize list of message handlers
  _handlers.Add(new TouchHandler());
            _handlers.Add(new DistanceHandler());
          _handlers.Add(new FacesHanler());
    _handlers.Add(new StateHandler());
        }

 public void Start()
  {
            _started = true;
 }

        public void Stop()
        {
        _started = false;
        }

        private async Task HandleRabbitMessageAsync(object sender, BasicDeliverEventArgs args)
        {
        foreach (var messageHandler in _handlers.TakeWhile(messageHandler => _publisher == null || !messageHandler.HandleRabbitMessage(_robotStateMachine, _publisher, sender, args)))
            {
            }
      await Task.CompletedTask;
  }
    }
}
