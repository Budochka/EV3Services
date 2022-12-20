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
					_ff.SetImage(message.body(), message.bodySize());
					_channel.ack(deliveryTag);

					_ff.FindFaces();
				}
			}
	);
}


void RabbitConsumer::Stop() const
{
    _channel.close();
}
