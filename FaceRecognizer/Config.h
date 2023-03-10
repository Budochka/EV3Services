#pragma once

class Config
{
private:
	std::string _logFileName = "";
    std::string _rabbitUserName = "guest";
    std::string _rabbitPassword = "guest";
    std::string _rabbitVHost = "/";
    std::string _rabbitHost = "localhost";
	std::string _face_recognition_set_path = "";
	std::string _known_faces_set_path = "";
	uint16_t _rabbitPort = 5672;

public:
	Config(const std::string fileName);

	[[nodiscard]] std::string log_file_name() const
	{
		return _logFileName;
	}

	[[nodiscard]] std::string rabbit_user_name() const
	{
		return _rabbitUserName;
	}

	[[nodiscard]] std::string rabbit_password() const
	{
		return _rabbitPassword;
	}

	[[nodiscard]] std::string rabbit_v_host() const
	{
		return _rabbitVHost;
	}

	[[nodiscard]] std::string rabbit_host() const
	{
		return _rabbitHost;
	}

	[[nodiscard]] uint16_t rabbit_port() const
	{
		return _rabbitPort;
	}

	[[nodiscard]] std::string faceRecognitionSetPath() const
	{
		return _face_recognition_set_path;
	}

	[[nodiscard]] std::string knownFacesSetPath() const
	{
		return _known_faces_set_path;
	}
};
