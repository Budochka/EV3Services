using RabbitMQ.Client.Events;

namespace Processor
{
    internal interface IMessageHandler
    {
        bool HandleRabbitMessage(RobotStateMachine sm, RabbitPublisher publisher, object sender, BasicDeliverEventArgs args);
    } 
}
