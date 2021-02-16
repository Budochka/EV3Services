#include "stdafx.h"
#include "Config.h"

using namespace boost::json;

Config::Config(const std::string fileName)
{
    std::ifstream t(fileName);
    std::string str((std::istreambuf_iterator<char>(t)),
        std::istreambuf_iterator<char>());

    error_code ec;
    value config = parse(str, ec);
	if (ec)
	{
        return;
	}

    object data = config.as_object();
	
    _logFileName = data["LogFileName"].as_string().c_str();
    _rabbitUserName = data["RabbitUserName"].as_string().c_str();
    _rabbitPassword = data["RabbitPassword"].as_string().c_str();
    _rabbitHost = data["RabbitHost"].as_string().c_str();
    _face_predictor_set_path = data["FacePredictorSet"].as_string().c_str();
    _shape_predictor_set_path = data["ShapePredictorSet"].as_string().c_str();
    _rabbitPort = data["RabbitPort"].as_int64();
}
