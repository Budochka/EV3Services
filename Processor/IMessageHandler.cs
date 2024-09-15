using RabbitMQ.Client.Events;

namespace Processor
{
    internal interface IMessageHandler
    {
        bool HandleRabbitMessage(WorldModel wold, RabbitPublisher publisher, object sender, BasicDeliverEventArgs args);
    } 
}
