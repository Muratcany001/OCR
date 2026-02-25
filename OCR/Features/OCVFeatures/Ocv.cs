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

    public static string OcvComprasion(string filePath)
    {
        // process file for before implement ocr
        Mat processedImage = ImageProcessing.ProcessFile(filePath);
        // call datamatrix reader 
        string dmResult = DatamatrixReader.ReadDataMatrix(filePath);
        string dmOutput;
        string ocrOutput;
        
        // parse gs1 output
        var items = Gs1Parser.Parse(dmResult);
        var dict = items.ToDictionary(x => x.AI, x => x.Value);
        var datamatrixDto = new DatamatrixEntity
        {
            Gtin = dict.GetValueOrDefault("01"),
            Sn = dict.GetValueOrDefault("21"),
            Lot = dict.GetValueOrDefault("10"),
            Man = dict.GetValueOrDefault("17"),
            ExpDate = dict.GetValueOrDefault("17")
        };
        dmOutput = $"{datamatrixDto.Gtin}{datamatrixDto.Sn}{datamatrixDto.Lot}{datamatrixDto.Man}{datamatrixDto.ExpDate}";
        
        // call ocr class
        string text = Ocr.Read(processedImage);
        
        if (dmResult != string.Empty)
        {
            var dmOcrEntity = new DatamatrixEntity
            {
                Gtin = Regex.Match(text, @"GTIN:\s*(\d{14})").Groups[1].Value,
                Sn = Regex.Match(text, @"SN:\s*([A-Za-z0-9]+)").Groups[1].Value,
                Lot = Regex.Match(text, @"LOT:\s*([A-Za-z0-9]+)").Groups[1].Value,
                Man = Regex.Match(text, @"MAN:\s*(\d{2}/\d{4})").Groups[1].Value,
                ExpDate = Regex.Match(text, @"EXP:\s*\(?(\d{2}/\d{4})\)?").Groups[1].Value
            };
            ocrOutput = $"{dmOcrEntity.Gtin}{dmOcrEntity.Sn}{dmOcrEntity.Lot}{dmOcrEntity.Man}{dmOcrEntity.ExpDate}";
        }
        else
        {
            var ocrEntity = new BoxEntity
            {
                BatchNo = Regex.Match(text, @"BatchNo.:\s*([A-Za-z0-9]+)").Groups[1].Value,
                MfgDate = Regex.Match(text, @"Mfg.Date:\s*(\d{2}/\d{4})").Groups[1].Value,
                ExpDate = Regex.Match(text, @"EXP\.Date:\s*\(?(\d{2}/\d{4})\)?").Groups[1].Value,
                Price = Regex.Match(text, @"Price\s*([0-9])").Groups[1].Value,
            };
            ocrOutput = $"{ocrEntity.BatchNo}{ocrEntity.MfgDate}{ocrEntity.ExpDate}{ocrEntity.Price}";
        }
        return ocrOutput;
    }
}