using Features.OCRFeatures.ImageProcessing;
using OCR.Entities;
using OCR.Features.DatamatrixFeatures;
using OCR.Helpers.OutputHelpers;
using OCR.Helpers.DatamatrixHelpers;
using OpenCvSharp;
namespace OCR.Features.OCVFeatures;

/// <summary>
/// OCR ve DataMatrix okuma işlemlerini yöneten ana sınıf.
/// Görsel tek seferde okunur, DatamatrixFinder tek seferde çalışır.
/// </summary>
public class Ocv
{
    private static CharacterRecognition Ocr;

    static Ocv()
    {
        Ocr = new CharacterRecognition("eng");
        using var dummyImage = new Mat(100, 100, MatType.CV_8UC1, Scalar.White);
        Ocr.Read(dummyImage);
        Console.WriteLine("Engine warmed up!");
    }
    /// <summary>
    /// Görselden OCR ve DataMatrix okuma sonuçlarını döndürür.
    /// </summary>
    public static OcvResultEntity.OcvResult AnalyzeImage(string filePath)
    {
        // wormup before works
        try
        {
        
        // 1. Görsel TEK SEFER okunur
        using Mat src = Cv2.ImRead(filePath);
        if (src.Empty())
        {
            Console.WriteLine("Image could not be loaded");
            return new OcvResultEntity.OcvResult { IsReadable = false };
        }
        
        // 2. DatamatrixFinder TEK SEFER çalışır
        var dmRect = DatamatrixFinder.FindDataMatrix(src);
        
        // 3. DataMatrix okuma (paylaşılan src + dmRect ile)
        string dmResult = DatamatrixReader.ReadDataMatrix(src, dmRect);
        bool hasDataMatrix = dmResult != string.Empty;
        
        // 4. Görsel işleme (paylaşılan src + dmRect ile — tekrar diskten okuma yok)
        Mat processedImage = ImageProcessing.ProcessFile(src, dmRect);
        
        // 5. OCR okuma
        string text = Ocr.Read(processedImage);
        processedImage.Dispose();
        
        // Datamatrix varsa datamatrix icerigini oku datamatrix yoksa ocr calistir.
        if (hasDataMatrix)
        {
            // GS1 parse
            var items = Gs1Parser.Parse(dmResult);
            var dict = items.ToDictionary(x => x.AI, x => x.Value);
            
            // OCR regex parse
            // Cikis turune gore karakter optimizasyonu
            var entity = new DatamatrixEntity
            {
                Gtin = dict.GetValueOrDefault("01"),
                // Ornek regex parser
                //Sn = RegexHelper.Sn.Match(text).Groups[1].Value,
                Sn = dict.GetValueOrDefault("21"),
                Lot = dict.GetValueOrDefault("10"),
                Man = dict.GetValueOrDefault("01"),
                ExpDate = dict.GetValueOrDefault("17")
            };
            
            return new OcvResultEntity.OcvResult
            {
                HasDataMatrix = true,
                IsReadable = !string.IsNullOrEmpty(entity.Gtin) && !string.IsNullOrEmpty(entity.Sn),
                DataMatrix = entity,
                RawOcrText = text
            };
        }
        else
        {
            // Null reference checks for Regexes
            var batchNoMatch = RegexHelper.BatchNo.Match(text);
            var mfgDateMatch = RegexHelper.MfgDate.Match(text);
            var expDateMatch = RegexHelper.ExpDate.Match(text);

            var entity = new BoxEntity
            {
                BatchNo = batchNoMatch.Success ? CharacterOptimizer.ToNumeric(batchNoMatch.Groups[1].Value) : string.Empty,
                MfgDate = mfgDateMatch.Success ? CharacterOptimizer.ToNumeric(mfgDateMatch.Groups[1].Value) : string.Empty,
                ExpDate = expDateMatch.Success ? CharacterOptimizer.ToNumeric(expDateMatch.Groups[1].Value) : string.Empty,
                // Price = Regex.Match(text, @"Price\s*([0-9]+)").Groups[1].Value,
            };
            
            return new OcvResultEntity.OcvResult
            {
                HasDataMatrix = false,
                IsReadable = !string.IsNullOrEmpty(entity.BatchNo),
                Box = entity,
                RawOcrText = text
            };
        }}
        catch (Exception e)
        {
            Console.WriteLine($"[OCV Error]: An error occurred while analyzing the image: {e.Message}");
            return new OcvResultEntity.OcvResult { IsReadable = false };
        }
    }
}

