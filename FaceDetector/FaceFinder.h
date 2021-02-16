#pragma once

using namespace dlib;

// ----------------------------------------------------------------------------------------
// Net for CNN face detection

template <long num_filters, typename SUBNET> using Con5d = con<num_filters, 5, 5, 2, 2, SUBNET>;
template <long num_filters, typename SUBNET> using Con5 = con<num_filters, 5, 5, 1, 1, SUBNET>;

template <typename SUBNET> using Downsampler = relu<affine<Con5d<32, relu<affine<Con5d<32, relu<affine<Con5d<16, SUBNET>>>>>>>>>;
template <typename SUBNET> using Rcon5 = relu<affine<Con5<45, SUBNET>>>;

using net_type = loss_mmod<con<1, 9, 9, 1, 1, Rcon5<Rcon5<Rcon5<Downsampler<input_rgb_image_pyramid<pyramid_down<6>>>>>>>>;

// ----------------------------------------------------------------------------------------

class FaceFinder
{
private:
	dlib::matrix<dlib::rgb_pixel> _image;
	std::vector<dlib::matrix<dlib::rgb_pixel>> _found_faces;
	net_type _detector_net;
	shape_predictor _sp;

public:
	FaceFinder(const std::string face_predictor_set_path, const std::string shape_predictor_set_path);

	//sets image from data in dng (dlib file format)
	void SetImage(const char* data, UINT size_of_image);

	//returns number of found faces
	UINT FindFaces();

	//returns image by index. Number of images is returned by FindFaces() function
	const dlib::matrix<dlib::rgb_pixel>* GetImage(UINT index);

	void PublishFaces(AMQP::Channel& channel);
};

