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
					if (_ff.NumberOfFaces() > 0)
					{
						_ff.PublishFaces(_channel);
					}
				}
			}
	);
}


void RabbitConsumer::Stop() const
{
    _channel.close();
}
