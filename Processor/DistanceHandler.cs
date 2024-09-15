using RabbitMQ.Client.Events;

namespace Processor;

class DistanceHandler : IMessageHandler
{
    public bool HandleRabbitMessage(WorldModel wold, RabbitPublisher publisher, object sender, BasicDeliverEventArgs args)
    {
        if (args.RoutingKey == "sensors.distance")
        {
            float distance = System.BitConverter.ToSingle(args.Body.ToArray());
            wold.Distance = distance;
            return true;
        }

        return false;
    }
}