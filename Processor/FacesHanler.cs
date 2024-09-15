using RabbitMQ.Client.Events;

namespace Processor;

class FacesHanler : IMessageHandler
{
    public bool HandleRabbitMessage(WorldModel wold, RabbitPublisher publisher, object sender,
        BasicDeliverEventArgs args)
    {
        if (args.RoutingKey == "faces.ids")
        {
            return true;
        }

        return false;
    }
}