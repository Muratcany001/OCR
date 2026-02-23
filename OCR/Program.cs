using System.Diagnostics;
using OpenCvSharp;

namespace OCR;

class Program
{
    private static readonly CharacterRecognition Ocr = new();
    
    static void Main(string[] args)
    {
        string filePath = "/Users/murat/RiderProjects/OCR/OCR/ExamplePhotos/dm4.bmp";
        Stopwatch sw = Stopwatch.StartNew();
        // process file for before implement ocr
        Mat processedImage = ImageProcessing.ProcessFile(filePath);
        
        // call ocr class
        string text = Ocr.Read(processedImage);
        // call datamatrix reader 
        string dmResult = DatamatrixReader.ReadDataMatrix(filePath);
        sw.Stop();
        Console.WriteLine(text);
        Console.WriteLine(dmResult);
        // Console.WriteLine($"Took {sw.ElapsedMilliseconds} ms");
    }
}