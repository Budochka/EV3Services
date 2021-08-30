using System.Threading;
using Emgu.CV;
using Emgu.CV.UI;

namespace VideoConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            VideoCapture videoCapture = new VideoCapture("rtmp://demo.flashphoner.com:1935/live/rtmp_dc4a");
            Mat frame = videoCapture.QueryFrame();

            while (frame != null)
            {
                ImageViewer.Show(frame, "Test Window");
                Thread.Sleep(100);
                frame = videoCapture.QueryFrame();
            }
        }
    }
}
