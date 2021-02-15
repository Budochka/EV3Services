#pragma once

class Config
{
private:
	std::string _logFileName = "";
    std::string _rabbitUserName = "guest";
    std::string _rabbitPassword = "guest";
    std::string _rabbitVHost = "/";
    std::string _rabbitHost = "localhost";
    uint16_t _rabbitPort = 5672;

public:
	Config(const std::string fileName);

	[[nodiscard]] std::string log_file_name() const
	{
		return _logFileName;
	}

	void set_log_file_name(std::string log_file_name)
	{
		_logFileName = log_file_name;
	}

	[[nodiscard]] std::string rabbit_user_name() const
	{
		return _rabbitUserName;
	}

	void set_rabbit_user_name(std::string rabbit_user_name)
	{
		_rabbitUserName = rabbit_user_name;
	}

	[[nodiscard]] std::string rabbit_password() const
	{
		return _rabbitPassword;
	}

	void set_rabbit_password(std::string rabbit_password)
	{
		_rabbitPassword = rabbit_password;
	}

	[[nodiscard]] std::string rabbit_v_host() const
	{
		return _rabbitVHost;
	}

	void set_rabbit_v_host(std::string rabbit_v_host)
	{
		_rabbitVHost = rabbit_v_host;
	}

	[[nodiscard]] std::string rabbit_host() const
	{
		return _rabbitHost;
	}

	void set_rabbit_host(std::string rabbit_host)
	{
		_rabbitHost = rabbit_host;
	}

	[[nodiscard]] uint16_t rabbit_port() const
	{
		return _rabbitPort;
	}

	void set_rabbit_port(uint16_t rabbit_port)
	{
		_rabbitPort = rabbit_port;
	}
};
