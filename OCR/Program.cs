using OpenCvSharp.Text;

namespace OCR;
using OpenCvSharp;
class Program
{
    static void Main(string[] args)
    {
        string filePath = "/Users/murat/RiderProjects/OCR/OCR/ExamplePhotos/DA3155805_202412181140099966.bmp";
        ImageProcessing.ProcessFile(filePath);
        
    }
}