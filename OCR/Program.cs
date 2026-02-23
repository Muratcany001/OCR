using System.Diagnostics;
using OCR.Features.OCVFeatures;
using OpenCvSharp;

namespace OCR;

class Program
{
    private static readonly CharacterRecognition Ocr = new();
    
    static void Main(string[] args)
    {
        string filePath = "/Users/murat/RiderProjects/OCR/OCR/ExamplePhotos/dm4.bmp";
        
        Ocv.OcvComprasion(filePath);

    }
}