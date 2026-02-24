using System.Diagnostics;
using System.Text.RegularExpressions;
using Features.OCRFeatures;
using Features.OCRFeatures.ImageProcessing;
using OCR.Entities;
using OCR.Features.DatamarixFeatures;
using OCR.Packages;
using OpenCvSharp;
namespace OCR.Features.OCVFeatures;

public class Ocv
{
    private static readonly CharacterRecognition Ocr = new();

    public static void OcvComprasion(string filePath)
    {
        // process file for before implement ocr
        Mat processedImage = ImageProcessing.ProcessFile(filePath);
        // call datamatrix reader 
        string dmResult = DatamatrixReader.ReadDataMatrix(filePath);
        
        // parse gs1 output
        var items = Gs1Parser.Parse(dmResult);
        var dict = items.ToDictionary(x => x.AI, x => x.Value);
        var DatamatrixDto = new DatamatrixEntity
        {
            Gtin = dict.GetValueOrDefault("01"),
            Sn = dict.GetValueOrDefault("21"),
            Lot = dict.GetValueOrDefault("10"),
            Man = dict.GetValueOrDefault("17")
        };
        Console.WriteLine("Datamatrix ciktilari");
        Console.WriteLine(DatamatrixDto.Gtin);
        Console.WriteLine(DatamatrixDto.Man);
        
        // call ocr class
        string text = Ocr.Read(processedImage);
        
        if (dmResult != string.Empty)
        {
            Console.WriteLine("if works");
            var dmOcrEntity = new DatamatrixEntity
            {
                Gtin = Regex.Match(text, @"GTIN:\s*(\d{14})").Groups[1].Value.ToUpper(),
                Sn = Regex.Match(text, @"SN:\s*([A-Za-z0-9]+)").Groups[1].Value.ToUpper(),
                Lot = Regex.Match(text, @"LOT:\s*([A-Za-z0-9]+)").Groups[1].Value.ToUpper(),
                Man = Regex.Match(text, @"MAN:\s*(\d{2}/\d{4})").Groups[1].Value.ToUpper(),
                ExpDate = Regex.Match(text, @"EXP:\s*\(?(\d{2}/\d{4})\)?").Groups[1].Value.ToUpper()
            };
            Console.WriteLine("Ocr ciktilari");
            Console.WriteLine(dmOcrEntity.ExpDate);
        }
        else
        {
            var ocrEntity = new BoxEntity
            {
                BatchNo = Regex.Match(text, @"BatchNo.:\s*([A-Za-z0-9]+)").Groups[1].Value.ToUpper(),
                MfgDate = Regex.Match(text, @"Mfg.Date:\s*(\d{2}/\d{4})").Groups[1].Value.ToUpper(),
                ExpDate = Regex.Match(text, @"EXP\.Date:\s*\(?(\d{2}/\d{4})\)?").Groups[1].Value.ToUpper(),
                Price = Regex.Match(text, @"Price:\s*([0-9])").Groups[1].Value.ToUpper(),
            };
            Console.WriteLine("Else works");
            Console.Write(ocrEntity.BatchNo);
            Console.Write(ocrEntity.ExpDate);
        }
    }
}