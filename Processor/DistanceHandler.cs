using RabbitMQ.Client.Events;

namespace Processor;

class DistanceHandler : IMessageHandler
{
    public bool HandleRabbitMessage(RobotStateMachine sm, RabbitPublisher publisher, object sender, BasicDeliverEventArgs args)
    {
        if (args.RoutingKey == "sensors.distance")
        {
            float distance = System.BitConverter.ToSingle(args.Body.ToArray());
            return true;
        }

        return false;
    }
}