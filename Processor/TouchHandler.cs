using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{
    class TouchHandler : IMessageHandler
    {
        public bool HandleRabbitMessage(RobotStateMachine stateMachine, RabbitPublisher publisher, object sender,
            BasicDeliverEventArgs args)
        {
            if (args.RoutingKey == "sensors.touch")
            {
                // Note: This method needs to be refactored to async, but keeping sync for now to minimize changes
                Task.Run(async () => await publisher?.PublishAsync("voice.text", Encoding.Unicode.GetBytes("Привет! Держи пять!")));
                AsyncEventingBasicConsumer ec = (AsyncEventingBasicConsumer)sender;
                Task.Run(async () => await ec.Channel.BasicAckAsync(args.DeliveryTag, false));

                return true;
            }

            return false;
        }
    }
}