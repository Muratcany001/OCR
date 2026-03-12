using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using OpenCvSharp;
using Features.OCRFeatures;
using Features.OCRFeatures.ImageProcessing;
using OCR.Entities;
using OCR.Features.DatamatrixFeatures;
using OCR.Helpers.OutputHelpers;
using OCR.Helpers.DatamatrixHelpers;

namespace OCR.Features.OCVFeatures
{
    /// <summary>
    /// Görüntüden OCR + DataMatrix (GS1) bilgilerini çeken sınıf.
    /// </summary>
    public class Ocv
    {
        private readonly DatamatrixReader _reader;
        private readonly DatamatrixFinder _finder;
        public static readonly CharacterRecognition Ocr = new();
    
        /// <summary>
        /// Görseli analiz eder ve <see cref="OcvResultEntity.OcvResult"/> döndürür.
        /// </summary>
        /// <param name="filePath">İşlenecek resim dosyasının tam yolu.</param>
        public OcvResultEntity.OcvResult AnalyzeImage(string filePath)
        {
            try
            {
                using var src = Cv2.ImRead(filePath, ImreadModes.Color);
                if (src.Empty())
                {
                    Console.WriteLine($"[OCV] Image could not be loaded: {filePath}");
                    return new OcvResultEntity.OcvResult { IsReadable = false };
                }
                Stopwatch sw = Stopwatch.StartNew();
                
                var dmRect = _finder.FindDataMatrix(src);
                bool dmRectIsValid = dmRect != default;
                
                // Mat image = src[dmRect];
                // Cv2.ImShow("123",image);
                // Cv2.WaitKey();
                DatamatrixEntity? dmEntity = null;
                bool hasDataMatrix = false;
                if (dmRectIsValid)
                {
                    var dmResult = _reader.ReadDataMatrix(src, dmRect);
                    sw.Stop();
                    Console.WriteLine($"[OCV] Analyze image time: {sw.ElapsedMilliseconds} ms");
                    hasDataMatrix = !string.IsNullOrWhiteSpace(dmResult);

                    if (hasDataMatrix)
                    {
                        var parsed = Gs1Parser.Parse(dmResult);
                        var dict   = parsed.ToDictionary(x => x.AI, x => x.Value);

                        dmEntity = new DatamatrixEntity
                        {
                            Gtin    = dict.GetValueOrDefault("01") ?? string.Empty,
                            Sn      = dict.GetValueOrDefault("21") ?? string.Empty,
                            Lot     = dict.GetValueOrDefault("10") ?? string.Empty,
                            ExpDate = dict.GetValueOrDefault("17")
                        };
                        
                    }
                    
                }

                string rawDmExp = dmEntity?.ExpDate;
                string formattedExp = null;
                if (!string.IsNullOrEmpty(rawDmExp) && rawDmExp.Length >= 4)
                {
                    // İlk 2 hane Yıl (28), sonraki 2 hane Ay (05)
                    // Bunları yer değiştirip birleştiriyoruz
                    formattedExp = rawDmExp.Substring(2, 2) + rawDmExp.Substring(0, 2);
                }
                
                var dmOutput = dmEntity != null
                    ? $"{dmEntity.Lot}{formattedExp}{dmEntity.Sn}"
                    : string.Empty;
                
                using var ocrMat = ImageProcessing.ProcessFile(src, dmRect);
                
                string rawOcrText = Ocr.Read(ocrMat);
                
                var ocrData = BuildOcrDataFromText(rawOcrText);
                
                var dmOcrOutput = $"{ocrData.Lot}{ocrData.ExpDate}{ocrData.Sn}";
                BoxEntity? box = null;
                bool hasBatchNo = false;
                if (!hasDataMatrix && (string.IsNullOrWhiteSpace(ocrData.Gtin) || string.IsNullOrWhiteSpace(ocrData.Sn)))
                {
                    box = BuildBoxFromText(rawOcrText);
                    hasBatchNo = box != null && !string.IsNullOrWhiteSpace(box.BatchNo);
                }
                var boxOutput = box != null ? $"{box.BatchNo}{box.MfgDate}{box.ExpDate}{box.Price}" 
                    : string.Empty;
                
                bool isReadable = false;

                if (hasDataMatrix && dmEntity != null)
                {
                    // DataMatrix geçerli ise GTIN ve SN dolu mu?
                    isReadable = !string.IsNullOrWhiteSpace(dmEntity.Gtin) &&
                                 !string.IsNullOrWhiteSpace(dmEntity.Sn);
                }
                else if (!string.IsNullOrWhiteSpace(ocrData.Gtin) && !string.IsNullOrWhiteSpace(ocrData.Sn))
                {
                    // DataMatrix yok, ama OCR'dan GTIN+SN var
                    isReadable = true;
                }
                else if (box != null && (!string.IsNullOrWhiteSpace(box.BatchNo) ||
                                         !string.IsNullOrWhiteSpace(box.ExpDate)))
                {
                    // Sadece Box bilgileri bulunduysa da okunabilir kabul edilebilir
                    isReadable = true;
                }
                
                return new OcvResultEntity.OcvResult
                {
                    HasDataMatrix = hasDataMatrix,
                    HasBatchNo    = hasBatchNo,
                    IsReadable    = isReadable,
                    DataMatrix    = dmEntity,
                    Box           = box,
                    RawOcrText    = rawOcrText,
                    OcrData       = ocrData,
                    DatamatrixOcrOutput = dmOcrOutput,
                    DatamatrixOutput =  dmOutput,
                    BoxOcrOutput = boxOutput
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OCV] Unexpected error: {ex.Message}");
                return new OcvResultEntity.OcvResult { IsReadable = false };
            }
           
        }
        
        private static DatamatrixEntity BuildOcrDataFromText(string text)
        {
            var gtinMatch = RegexHelper.Gtin.Match(text);
            var snMatch   = RegexHelper.Sn.Match(text);
            var lotMatch  = RegexHelper.Lot.Match(text);
            var expMatch  = RegexHelper.Exp.Match(text);
            
            return new DatamatrixEntity
            {
                Gtin    = gtinMatch.Success ? gtinMatch.Groups[1].Value : string.Empty,
                Sn      = snMatch.Success   ? snMatch.Groups[1].Value   : string.Empty,
                Lot     = lotMatch.Success  ? lotMatch.Groups[1].Value  : string.Empty,
                ExpDate = expMatch.Success  ? expMatch.Result("$1$2")  : string.Empty
            };
        }
        
        private static BoxEntity? BuildBoxFromText(string text)
        {
            var batchMatch = RegexHelper.BatchNo.Match(text);
            var dateMatch  = RegexHelper.DoubleDate.Match(text);
            var priceMatch = RegexHelper.Price.Match(text);
            
            // Hiçbir şey bulunamadıysa null dön — boş entity üretilmesin
            if (!batchMatch.Success && !dateMatch.Success && !priceMatch.Success)
                return null;
            
            string mfgDate = string.Empty;
            string expDate = string.Empty;
            
            if (dateMatch.Success)
            {
                mfgDate = $"{dateMatch.Groups[1].Value}/{dateMatch.Groups[2].Value}";
                expDate = $"{dateMatch.Groups[3].Value}/{dateMatch.Groups[4].Value}";
            }
            
             return new BoxEntity
            {
                BatchNo = batchMatch.Success ? batchMatch.Groups[1].Value : string.Empty,
                MfgDate = mfgDate,
                ExpDate = expDate,
                Price   = priceMatch.Success ? priceMatch.Groups[1].Value : string.Empty
            };
        }
    }
}