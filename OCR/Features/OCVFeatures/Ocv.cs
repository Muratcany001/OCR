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
        var boxEntity = new DatamatrixEntity
        {
            Gtin = Regex.Match(text, @"GTIN:\s*(\d{14})").Groups[1].Value,
            Sn =  Regex.Match(text, @"SN:\s*([A-Za-z0-9]+)").Groups[1].Value,
            Lot =   Regex.Match(text, @"LOT:\s*([A-Za-z0-9]+)").Groups[1].Value,
            Man = Regex.Match(text, @"MAN:\s*(\d{2}/\d{4})").Groups[1].Value,
            ExpDate = Regex.Match(text, @"EXP:\s*(\d{2}/\d{4})").Groups[1].Value
        };
        Console.WriteLine(boxEntity.Lot);
        Console.WriteLine(boxEntity.Sn);
        Console.WriteLine(boxEntity.Gtin);
        Console.WriteLine(boxEntity.Man);
        Console.WriteLine(boxEntity.ExpDate);
        
        // parse gs1 output
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