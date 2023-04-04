#include "stdafx.h"
#include "RabbitConsumer.h"

using namespace std;

void RabbitConsumer::Run() const
{
	_channel.consume("images")
		.onReceived(
			[this](const AMQP::Message& message,
				uint64_t deliveryTag,
				bool redelivered)
			{
				if (!redelivered)
				{
					_fr.SetImage(message.body(), message.bodySize());
					string name = _fr.ReconizedFace();
					if (!name.empty())
					{
						_channel.publish("EV3", "text.face_name", name.c_str(), name.size());
					}
					_channel.ack(deliveryTag);
				}
			}
	);
}


void RabbitConsumer::Stop() const
{
    _channel.close();
}
