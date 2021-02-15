// FaceDetector.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "stdafx.h"

#include "Config.h"
#include "SimplePocoHandler.h"

using namespace std;

int main()
{
    Config cfg("config.json");
	
    SimplePocoHandler handler(cfg.rabbit_host(), cfg.rabbit_port());

    AMQP::Connection connection(&handler, AMQP::Login(cfg.rabbit_user_name(), cfg.rabbit_password()), cfg.rabbit_v_host());

    AMQP::Channel channel(&connection);
    auto receiveMessageCallback = [](const AMQP::Message& message,
        uint64_t deliveryTag,
        bool redelivered)
    {
        cout << " [x] " << message.body() << endl;
    };

    AMQP::QueueCallback callback =
        [&](const std::string& name, int msgcount, int consumercount)
    {
        channel.bindQueue("logs", name, "");
        channel.consume(name, AMQP::noack).onReceived(receiveMessageCallback);
    };

    AMQP::SuccessCallback success = [&]()
    {
        channel.declareQueue(AMQP::exclusive).onSuccess(callback);
    };

    channel.declareExchange("logs", AMQP::fanout).onSuccess(success);

    std::cout << " [*] Waiting for messages. To exit press CTRL-C\n";
    handler.loop();

    return 0;
}
