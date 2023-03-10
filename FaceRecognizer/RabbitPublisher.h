#pragma once

class RabbitPublisher
{
public:
	RabbitPublisher(AMQP::Channel& channel)
		:_channel(channel)
	{
	}

	void Run();
	void Stop();

private:
	AMQP::Channel& _channel;
};

