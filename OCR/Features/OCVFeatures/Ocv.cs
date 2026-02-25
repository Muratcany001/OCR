using System.Diagnostics;
using System.Text.RegularExpressions;
using Features.OCRFeatures;
using Features.OCRFeatures.ImageProcessing;
using OCR.Entities;
using OCR.Features.DatamarixFeatures;
using OCR.Packages;
using OpenCvSharp;
namespace OCR.Features.OCVFeatures;

/// <summary>
/// OCR ve DataMatrix okuma işlemlerini yöneten ana sınıf.
/// Görsel işleme, DataMatrix okuma ve Regex tabanlı metin ayrıştırma süreçlerini koordine eder.
/// </summary>
public class Ocv
{
    private static readonly CharacterRecognition Ocr = new();

    /// <summary>
    /// Görselden OCR ve DataMatrix okuma sonuçlarını döndürür.
    /// DataMatrix varsa DatamatrixEntity, yoksa BoxEntity döner.
    /// </summary>
    /// <param name="filePath">Okunacak görselin dosya yolu.</param>
    /// <returns>
    /// HasDataMatrix: DataMatrix bulundu mu,
    /// DataMatrix: DataMatrix entity (null olabilir),
    /// Box: Box entity (null olabilir),
    /// RawOcrText: Ham OCR çıktısı (debug için)
    /// </returns>
    public static OcvResult OcvComprasion(string filePath)
    {
        // Görsel işleme
        Mat processedImage = ImageProcessing.ProcessFile(filePath);
        
        // DataMatrix okuma
        string dmResult = DatamatrixReader.ReadDataMatrix(filePath);
        bool hasDataMatrix = dmResult != string.Empty;
        
        // OCR okuma
        string text = Ocr.Read(processedImage);
        
        if (hasDataMatrix)
        {
            // GS1 parse
            var items = Gs1Parser.Parse(dmResult);
            var dict = items.ToDictionary(x => x.AI, x => x.Value);
            
            // OCR regex parse
            var entity = new DatamatrixEntity
            {
                Gtin = Regex.Match(text, @"GTIN:\s*(\d{14})").Groups[1].Value,
                Sn = Regex.Match(text, @"SN:\s*([A-Za-z0-9]+)").Groups[1].Value,
                Lot = Regex.Match(text, @"LOT:\s*([A-Za-z0-9]+)").Groups[1].Value,
                Man = Regex.Match(text, @"MAN:\s*(\d{2}/\d{4})").Groups[1].Value,
                ExpDate = Regex.Match(text, @"EXP:\s*\(?(\d{2}/\d{4})\)?").Groups[1].Value
            };
            
            // OCR'dan bulunamayan alanları GS1 verisinden doldur
            if (string.IsNullOrEmpty(entity.Gtin)) entity.Gtin = dict.GetValueOrDefault("01");
            if (string.IsNullOrEmpty(entity.Sn)) entity.Sn = dict.GetValueOrDefault("21");
            if (string.IsNullOrEmpty(entity.Lot)) entity.Lot = dict.GetValueOrDefault("10");
            if (string.IsNullOrEmpty(entity.Man)) entity.Man = dict.GetValueOrDefault("17");
            if (string.IsNullOrEmpty(entity.ExpDate)) entity.ExpDate = dict.GetValueOrDefault("17");
            
            return new OcvResult
            {
                HasDataMatrix = true,
                DataMatrix = entity,
                RawOcrText = text
            };
        }
        else
        {
            var entity = new BoxEntity
            {
                BatchNo = Regex.Match(text, @"BatchNo.:\s*([A-Za-z0-9]+)").Groups[1].Value,
                MfgDate = Regex.Match(text, @"Mfg.Date:\s*(\d{2}/\d{4})").Groups[1].Value,
                ExpDate = Regex.Match(text, @"EXP\.Date:\s*\(?(\d{2}/\d{4})\)?").Groups[1].Value,
                // Price = Regex.Match(text, @"Price\s*([0-9]+)").Groups[1].Value,
            };
            
            return new OcvResult
            {
                HasDataMatrix = false,
                Box = entity,
                RawOcrText = text
            };
        }
    }
}

/// <summary>
/// OcvComprasion metodunun sonuç sınıfı.
/// DataMatrix veya Box entity'sini ve metadata bilgilerini taşır.
/// </summary>
public class OcvResult
{
    public bool HasDataMatrix { get; set; }
    public DatamatrixEntity? DataMatrix { get; set; }
    public BoxEntity? Box { get; set; }
    public string RawOcrText { get; set; } = "";
}