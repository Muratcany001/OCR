using System.Text.RegularExpressions;
using OCR.Entities;
using OCR.Packages;
using OpenCvSharp;
using SimpleSoft.Gs1Parser;
using Gs1Parser = SimpleSoft.Gs1Parser.Gs1Parser;

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
        
        //

        var items =DatamatrixHelper.Parse(dmResult);
        var dict = items.ToDictionary(x => x.AI, x => x.Value);
        var entity = new DatamatrixEntity
        {
            Gtin = dict.GetValueOrDefault("01"),
            Sn   = dict.GetValueOrDefault("21"),
            Lot  = dict.GetValueOrDefault("10"),
            Man  = dict.GetValueOrDefault("17")
        };
        Console.WriteLine(entity.Gtin);
    }
}