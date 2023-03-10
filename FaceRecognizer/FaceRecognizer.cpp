#include "stdafx.h"
#include "FaceRecognizer.h"

FaceRecognizer::FaceRecognizer(const string trained_faces_set, const string human_face_detector_net)
{
    //initialize face descriptors net
    deserialize(human_face_detector_net) >> _descriptors_net;

    //load known faces descriptors
    Document face_data;

    try
    {
        ifstream ifs(trained_faces_set);
        IStreamWrapper isw(ifs);

        face_data.ParseStream(isw);
    }
    catch (exception& e)
    {
        throw e;
    }

    if (!face_data.IsArray())
    {
        throw exception("Wrong file format");
    }


    for (auto itr = face_data.Begin(); itr != face_data.End(); ++itr)
    {
        const Value& value = *itr;
        const string name = value[0].GetString();
        auto face_vector = value[1].GetArray();

        auto element = _knownFaces.insert(pair<string, matrix<float, 0, 1>>(name, matrix<float, 0, 1>()));
        std::vector<float> tmp;
        for (auto itr1 = face_vector.Begin(); itr1 != face_vector.End(); ++itr1)
        {
            tmp.push_back((*itr1).GetFloat());
        }
        element->second = mat(tmp);
        tmp.clear();
    }
}

//sets image from data in dng (dlib file format)
void FaceRecognizer::SetImage(const char* data, UINT size_of_image)
{
    matrix<rgb_pixel> image;

    if (data != nullptr)
    {

//        load_dng(_image, );
        _face_descriptors = _descriptors_net(image);
    }
}

string FaceRecognizer::ReconizedFace()
{
    std::map<string, int> voting;

    for (auto&& known_face : _knownFaces)
    {
        if (length(_face_descriptors - known_face.second) < THRESHOLD)
        {
            auto it = voting.find(known_face.first);
            if (it != voting.end())
            {
                it->second++;
            }
            else
            {
                voting.insert(pair<string, int>(known_face.first, 1));
            }
        }
    }

    if (voting.size() > 0)
    {
        auto max = std::max_element(voting.begin(), voting.end(), [](const pair<string, int>& a, const pair<string, int>& b)->bool { return a.second < b.second; });
        return max->first;
    }
    else
    {
        return "";
    }
}
