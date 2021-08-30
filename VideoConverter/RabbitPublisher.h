#pragma once

class RabbitPublisher
{
public:
	RabbitPublisher(AMQP::Channel& channel)
		:_channel(channel)
	{
	}

private:
	AMQP::Channel& _channel;
};

