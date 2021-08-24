#include "stdafx.h"
#include "RabbitConsumer.h"

using namespace std;

void RabbitConsumer::Run()
{
    auto receiveMessageCallback = [this](const AMQP::Message& message,
        uint64_t deliveryTag,
        bool redelivered)
    {
        if (!redelivered)
        {
            _ff.SetImage(message.body(), message.size());
            _ff.FindFaces();
        }
    };

    AMQP::QueueCallback callback =
        [&](const std::string& name, int msgcount, int consumercount)
    {
        _channel.bindQueue("EV3", "images.general", "");
        _channel.consume(name, AMQP::noack).onReceived(receiveMessageCallback);
    };

    AMQP::SuccessCallback success = [&]()
    {
        _channel.declareQueue(AMQP::exclusive).onSuccess(callback);
    };

    _channel.declareExchange("logs", AMQP::fanout).onSuccess(success);
}


void RabbitConsumer::Stop()
{
    _channel.close();
}
