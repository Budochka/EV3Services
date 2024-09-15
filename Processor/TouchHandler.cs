using RabbitMQ.Client.Events;
using System.Text;

namespace Processor
{
    class TouchHandler : IMessageHandler
    {
        public bool HandleRabbitMessage(WorldModel wold, RabbitPublisher publisher, object sender,
            BasicDeliverEventArgs args)
        {
            if (args.RoutingKey == "sensors.touch")
            {
                publisher?.Publish("voice.text", Encoding.Unicode.GetBytes("Привет! Держи пять!"));
                EventingBasicConsumer ec = (EventingBasicConsumer)sender;
                ec.Model.BasicAck(args.DeliveryTag, false);

                return true;
            }

            return false;
        }
    }
}