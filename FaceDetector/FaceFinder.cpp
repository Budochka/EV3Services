#include "stdafx.h"
#include "FaceFinder.h"

FaceFinder::FaceFinder(const std::string face_predictor_set_path, const std::string shape_predictor_set_path)
{
	deserialize(face_predictor_set_path) >> _detector_net;
	deserialize(shape_predictor_set_path) >> _sp;
}

void FaceFinder::SetImage(const char* data, const UINT size_of_image)
{
	if (data != nullptr)
	{
		load_jpeg(_image, data, size_of_image);
	}
}

//returns number of found faces
UINT FaceFinder::FindFaces()
{
	_found_faces.clear();
	
	for (auto&& face : _detector_net(_image))
	{
		auto shape = _sp(_image, face);

		matrix<rgb_pixel> face_chip;
		extract_image_chip(_image, get_face_chip_details(shape, 150, 0.25), face_chip);
		_found_faces.push_back(std::move(face_chip));
	}

	return _found_faces.size();
}

//returns image by index. Number of images is returned by FindFaces() function
const dlib::matrix<dlib::rgb_pixel>* FaceFinder::GetImage(UINT index) const
{
	if (index < _found_faces.size())
		return &_found_faces[index];
	return nullptr;
}

void FaceFinder::PublishFaces(AMQP::Channel& channel)
{
	std::vector<char> v;
	for (auto it = _found_faces.begin(); it != _found_faces.end(); ++it)
	{
		v.clear();

		//convert image to bytes using streams
		boost::iostreams::back_insert_device<std::vector<char>> sink{ v };
		boost::iostreams::stream<boost::iostreams::back_insert_device<std::vector<char>>> os{ sink };

		save_dng(*it, os);
		channel.publish("EV3", "images.face", v.data(), v.size());
	}
}
