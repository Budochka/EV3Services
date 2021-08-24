using System;
using Emgu.CV;


namespace VideoConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            VideoCapture videoCapture = new VideoCapture(0);
            var frame = videoCapture.QueryFrame();

            while (frame != null)
            {
                using (frame)
                {
                }
            }
        }
    }
}
