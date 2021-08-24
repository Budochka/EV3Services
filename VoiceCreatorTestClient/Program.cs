using System;
using System.IO;
using System.Media;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace VoiceCreatorTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory()

            {
                UserName = "guest",
                Password = "guest",
                HostName = "localhost",
                Port = 5672,
                AutomaticRecoveryEnabled = true
            };
            var connection = factory.CreateConnection();
            if (connection == null)
                return;

            var channel = connection.CreateModel();
            if (channel == null)
                return;

            channel.ExchangeDeclare(exchange: "EV3", type: "topic", autoDelete: true);

            var queueVoice = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueVoice, exchange: "EV3", routingKey: "voice.generated.wav");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var wavStream = new MemoryStream(body);
                SoundPlayer sp = new SoundPlayer(wavStream);
                sp.Play();
            };
            channel.BasicConsume(queue: queueVoice, autoAck: false, consumer: consumer);

            bool stopped = false;
            while (!stopped)
            {
                Console.WriteLine("Введите текст, done для выхода");

                var str = Console.ReadLine();
                if (str == "done")
                {
                    stopped = true;
                    continue;
                }

                byte[] bytes = new byte[str.Length * sizeof(char)];
                System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);

                channel.BasicPublish(exchange: "EV3",
                                     routingKey: "voice.text",
                                     basicProperties: null,
                                     body: bytes);
            }
        }
    }
}
