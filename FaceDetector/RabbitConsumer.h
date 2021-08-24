#pragma once

#include "FaceFinder.h"
#include "Config.h"

class RabbitConsumer
{
public:
	RabbitConsumer(AMQP::Channel& channel, FaceFinder& ff)
		:_channel(channel), _ff(ff)
	{
	}

	void Run();
	void Stop();

private:
	AMQP::Channel& _channel;
	FaceFinder& _ff;
};

