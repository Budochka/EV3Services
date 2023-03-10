#pragma once

#define NOMINMAX

#include <iostream>
#include <memory>
#include <amqpcpp.h>
#include <vector>
#include <thread>
#include <chrono>
#include <cstring>
#include <cassert>
#include <iostream>
#include <fstream>

#include <boost/json.hpp>
#include <boost/iostreams/device/back_inserter.hpp>
#include <boost/iostreams/stream.hpp>
#include <boost/log/sources/logger.hpp>
#include <boost/log/sources/record_ostream.hpp>
#include <boost/log/sources/global_logger_storage.hpp>
#include <boost/log/utility/setup/file.hpp>
#include <boost/log/utility/setup/common_attributes.hpp>

namespace logging = boost::log;
namespace src = boost::log::sources;

#include <rapidjson/document.h>
#include <rapidjson/istreamwrapper.h>

#include <Poco/Net/StreamSocket.h>

#include <opencv2/opencv.hpp>

#include <dlib/dnn.h>
#include <dlib/opencv.h>
#include <dlib/image_io.h>
#include <dlib/image_processing/frontal_face_detector.h>
#include <dlib/image_processing/render_face_detections.h>
#include <dlib/image_processing.h>
