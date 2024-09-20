using NLog;
using RabbitMQ.Client.Events;

namespace VoiceCreator
{
    class Worker
    {
        private readonly Logger _logs;
        private RabbitConsumer? _consumer;
        private RabbitPublisher? _publisher;
        private readonly Config? _config;

        private bool _started;

        public Worker(Logger log, Config? config)
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

            _publisher = new RabbitPublisher(_logs);
            _publisher.ConnectToRabbit(_config.RabbitUserName,
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

        private void HandleRabbitMessage(object sender, BasicDeliverEventArgs args)
        {
            var bytes = args.Body.ToArray();
            if ((bytes.Length > 0) && _started)
            {
                char[] chars = new char[bytes.Length / sizeof(char)];
                Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);

                var httpClient = new HttpClient();
                var openTTSClient = new OpenTTS(httpClient);
                openTTSClient.TtsAsync("larynx:nikolaev-glow_tts", new string(chars), Vocoder.High, 0.005, false).Wait();
                //                openTTSClient.TtsAsync("espeak: Russian", new string(chars), null, null, false).Wait();
                if (openTTSClient.IsSuccess)
                {
                    _publisher?.Publish(openTTSClient.LastResponse);
                }
            }

            EventingBasicConsumer ec = (EventingBasicConsumer)sender;
            ec.Model.BasicAck(args.DeliveryTag, false);
        }
    }
}
