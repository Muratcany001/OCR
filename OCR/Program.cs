using System.Diagnostics;
using OCR.Features.OCVFeatures;
using OCR.Helpers.OutputHelpers;

namespace OCR;

class Program
{
    static void Main(string[] args)
    {
        string filePath = "/Users/murat/RiderProjects/OCR/OCR/ExamplePhotos/test.png";
        var message = Ocv.OcvComprasion(filePath);
        Console.WriteLine(message);
        if (message.Length < 19)
        {
            var parsedMessage =OutputParser.OCRParse(message);    
            Console.WriteLine(parsedMessage.BatchNo);
        }
        else
        {
            var parsedMessage = OutputParser.DatamatrixParse(message);
            Console.WriteLine(parsedMessage.Gtin);
        }
        
    }
}