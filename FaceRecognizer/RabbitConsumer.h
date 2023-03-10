#pragma once

#include "FaceRecognizer.h"

class RabbitConsumer
{
public:
	RabbitConsumer(AMQP::Channel& channel, FaceRecognizer& fr)
		:_channel(channel), _fr(fr)
	{
	}

	void Run() const;
	void Stop() const;

private:
	AMQP::Channel& _channel;
	FaceRecognizer& _fr;
};

