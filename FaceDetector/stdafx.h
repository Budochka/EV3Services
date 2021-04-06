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
#include <boost/iostreams/device/array.hpp>
#include <boost/iostreams/stream.hpp>
#include <boost/iostreams/device/back_inserter.hpp>
#include <boost/iostreams/stream.hpp>
#include <boost/log/sources/global_logger_storage.hpp>

namespace logging = boost::log;
namespace src = boost::log::sources;
namespace keywords = boost::log::keywords;

#include <Poco/Net/StreamSocket.h>

#include <opencv2/opencv.hpp>

#include <dlib/dnn.h>
#include <dlib/opencv.h>
#include <dlib/image_io.h>
#include <dlib/image_processing/frontal_face_detector.h>
#include <dlib/image_processing/render_face_detections.h>
#include <dlib/image_processing.h>
