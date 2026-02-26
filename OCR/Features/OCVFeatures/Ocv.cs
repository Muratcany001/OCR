using System.Diagnostics;
using System.Text.RegularExpressions;
using Features.OCRFeatures;
using Features.OCRFeatures.ImageProcessing;
using OCR.Entities;
using OCR.Features.DatamarixFeatures;
using OCR.Helpers.OutputHelpers;
using OCR.Packages;
using OpenCvSharp;
namespace OCR.Features.OCVFeatures;

/// <summary>
/// OCR ve DataMatrix okuma işlemlerini yöneten ana sınıf.
/// Görsel tek seferde okunur, DatamatrixFinder tek seferde çalışır.
/// </summary>
public class Ocv
{
    private static readonly CharacterRecognition Ocr = new();

    /// <summary>
    /// Görselden OCR ve DataMatrix okuma sonuçlarını döndürür.
    /// </summary>
    public static OcvResultEntity.OcvResult OcvComprasion(string filePath)
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
        
        if (hasDataMatrix)
        {
            // GS1 parse — DataMatrix decode edilmişse regex'e gerek yok, doğrudan GS1 kullan
            var items = Gs1Parser.Parse(dmResult);
            var dict = items.ToDictionary(x => x.AI, x => x.Value);
            
            var entity = new DatamatrixEntity
            {
                Gtin    = dict.GetValueOrDefault("01") ?? string.Empty,
                Sn      = dict.GetValueOrDefault("21") ?? string.Empty,
                Lot     = dict.GetValueOrDefault("10") ?? string.Empty,
                Man     = dict.GetValueOrDefault("11") ?? string.Empty,
                ExpDate = dict.GetValueOrDefault("17") ?? string.Empty,
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
            // DataMatrix decode olmasa da OCR metnine bakarak label formatını tespit et
            bool isDatamatrixLabel = text.Contains("LOT:") || text.Contains("GTIN:");

            if (isDatamatrixLabel)
            {
                // DataMatrix etiket formatı — barcod okunamadı ama yazı okunabildi
                var entity = new DatamatrixEntity
                {
                    Gtin    = RegexHelper.Gtin.Match(text).Groups[1].Value,
                    Sn      = RegexHelper.Sn.Match(text).Groups[1].Value,
                    Lot     = RegexHelper.Lot.Match(text).Groups[1].Value,
                    Man     = RegexHelper.Man.Match(text).Groups[1].Value,
                    ExpDate = RegexHelper.Exp.Match(text).Groups[1].Value,
                };

                return new OcvResultEntity.OcvResult
                {
                    HasDataMatrix = false,
                    IsReadable = !string.IsNullOrEmpty(entity.Gtin) && !string.IsNullOrEmpty(entity.Sn),
                    DataMatrix = entity,
                    RawOcrText = text
                };
            }
            else
            {
                // Box etiket formatı (Batch No / Mfg.Date / EXP.Date)
                var entity = new BoxEntity
                {
                    BatchNo = RegexHelper.BatchNo.Match(text).Groups[1].Value,
                    MfgDate = RegexHelper.MfgDate.Match(text).Groups[1].Value,
                    ExpDate = RegexHelper.ExpDate.Match(text).Groups[1].Value,
                };

                return new OcvResultEntity.OcvResult
                {
                    HasDataMatrix = false,
                    IsReadable = !string.IsNullOrEmpty(entity.BatchNo),
                    Box = entity,
                    RawOcrText = text
                };
            }
        }
    }
}