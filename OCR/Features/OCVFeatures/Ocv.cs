using System.Text.RegularExpressions;
using OpenCvSharp;

namespace OCR.Features.OCVFeatures;

public class Ocv
{
    private static readonly CharacterRecognition Ocr = new();
    public static void OcvComprasion(string filePath)
    {
        // process file for before implement ocr
        Mat processedImage = ImageProcessing.ProcessFile(filePath);
        // call ocr class
        string text = Ocr.Read(processedImage);
        // call datamatrix reader 
        string dmResult = DatamatrixReader.ReadDataMatrix(filePath);
        
        Console.WriteLine(dmResult);
    }
}