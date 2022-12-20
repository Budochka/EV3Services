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

	void Run() const;
	void Stop() const;

private:
	AMQP::Channel& _channel;
	FaceFinder& _ff;
};

