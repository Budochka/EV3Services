﻿using RabbitMQ.Client.Events;

namespace Processor;

class FacesHanler : IMessageHandler
{
    public bool HandleRabbitMessage(RobotStateMachine sm, RabbitPublisher publisher, object sender,
        BasicDeliverEventArgs args)
    {
        if (args.RoutingKey == "faces.ids")
        {
            return true;
        }

        return false;
    }
}