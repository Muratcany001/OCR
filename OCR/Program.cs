using System.Diagnostics;
using OCR.Features.OCVFeatures;
using OCR.Helpers.OutputHelpers;

namespace OCR;

class Program
{
    static void Main(string[] args)
    {
        string filePath = "/Users/murat/RiderProjects/OCR/OCR/ExamplePhotos/dm4.bmp";
        var message = Ocv.OcvComprasion(filePath);
        Console.WriteLine(message.Gtin);
    }
}