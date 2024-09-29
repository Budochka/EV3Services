using RabbitMQ.Client.Events;

namespace Processor;

class StateHandler : IMessageHandler
{
    public bool HandleRabbitMessage(RobotStateMachine sm, RabbitPublisher publisher, object sender,
        BasicDeliverEventArgs args)
    {
        var key = args.RoutingKey;

        if ((key == "state.greet") || (key == "state.direct") || (key == "state.explore"))
        {
            sm.SetState(key);
            return true;
        }

        return false;
    }
}