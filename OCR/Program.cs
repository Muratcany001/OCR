using System.Diagnostics;
using OpenCvSharp;

namespace OCR;

class Program
{
    private static readonly CharacterRecognition Ocr = new();
    
    static void Main(string[] args)
    {
        string filePath = "/Users/murat/RiderProjects/OCR/OCR/ExamplePhotos/dm1.bmp";
        
        Mat processedImage = ImageProcessing.ProcessFile(filePath);
        
        Stopwatch sw = Stopwatch.StartNew();
        string text = Ocr.Read(processedImage);
        sw.Stop();
        
        Console.WriteLine(text);
        Console.WriteLine($"Took {sw.ElapsedMilliseconds} ms");
    }
}