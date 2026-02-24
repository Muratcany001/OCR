using System.Diagnostics;
using OCR.Features.OCVFeatures;

namespace OCR;

class Program
{
    static void Main(string[] args)
    {
        string filePath = "/Users/murat/RiderProjects/OCR/OCR/ExamplePhotos/dm4.bmp";
        Ocv.OcvComprasion(filePath);
    }
}