// FaceDetector.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "stdafx.h"
#include "stdafx.h"

#include "Config.h"
#include "SimplePocoHandler.h"
#include "RabbitConsumer.h"

using namespace std;

BOOST_LOG_INLINE_GLOBAL_LOGGER_DEFAULT(my_logger, src::logger_mt);

int main()
{
    Config cfg("config.json");

    SimplePocoHandler handler(cfg.rabbit_host(), cfg.rabbit_port());
    AMQP::Connection connection(&handler, AMQP::Login(cfg.rabbit_user_name(), cfg.rabbit_password()), cfg.rabbit_v_host());
    AMQP::Channel channel(&connection);
    channel.onError([&handler](const char* message)
        {
            std::cout << "Channel error: " << message << std::endl; 
			handler.quit();
        });
    
    channel.declareExchange("EV3", AMQP::topic, AMQP::autodelete);
    channel.declareQueue("images", AMQP::exclusive);
    channel.bindQueue("EV3", "images", "images.general");

    FaceFinder ff(cfg.facePredictorSetPath(), cfg.shapePredictorSetPath());

    RabbitConsumer consumer(channel, ff);
    consumer.Run();

    std::cout << " [*] Waiting for messages. To exit press CTRL-C\n";
    handler.loop();

    return 0;
}
