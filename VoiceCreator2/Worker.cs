using NLog;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;
using EV3Services.Common;

namespace VoiceCreator
{
    class Worker
    {
        private readonly Logger _logs;
        private RabbitMQConsumer? _consumer;
        private RabbitMQPublisher? _publisher;
        private readonly Config? _config;

        private bool _started;

        public Worker(Logger log, Config? config)
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
                routingKeys: new[] { "voice.text" },
                autoAck: false);
            _logs.Info("RabbitConsumer created");

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
            _started = true;
        }

        public void Stop()
        {
            _started = false;
        }

        private async Task HandleRabbitMessageAsync(object sender, BasicDeliverEventArgs args)
        {
            var bytes = args.Body.ToArray();
            if ((bytes.Length > 0) && _started)
            {
                char[] chars = new char[bytes.Length / sizeof(char)];
                Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);

                if (!string.IsNullOrEmpty(_config?.YandexApiKey))
                {
                    var yandexClient = new YandexSpeechKit(_config.YandexApiKey, 
                        voice: "filipp", language: "ru-RU", emotion: "neutral", speed: 1.0);
                    await yandexClient.SynthesizeAsync(new string(chars));
                    if (yandexClient.IsSuccess && _publisher != null)
                    {
                        await _publisher.PublishAsync("voice.wav", yandexClient.LastResponse);
                    }
                }
            }

            AsyncEventingBasicConsumer ec = (AsyncEventingBasicConsumer)sender;
            await ec.Channel.BasicAckAsync(args.DeliveryTag, false);
        }
    }
}
