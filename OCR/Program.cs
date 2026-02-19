using System.Diagnostics;
using OpenCvSharp;

namespace OCR;

class Program
{
    private static readonly CharacterRecognition Ocr = new();
    
    static void Main(string[] args)
    {
        string filePath = "/Users/murat/RiderProjects/OCR/OCR/ExamplePhotos/DA3155805_202412181150384510.bmp";
        
        Mat processedImage = ImageProcessing.ProcessFile(filePath);
        
        Stopwatch sw = Stopwatch.StartNew();
        string text = Ocr.Read(processedImage);
        sw.Stop();
        
        Console.WriteLine(text);
        Console.WriteLine($"Took {sw.ElapsedMilliseconds} ms");
    }
}