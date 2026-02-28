using System;
using System.Collections.Generic;
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
    public static class Ocv
    {
        private static readonly CharacterRecognition Ocr = new();

        /// <summary>
        /// Görseli analiz eder ve <see cref="OcvResultEntity.OcvResult"/> döndürür.
        /// </summary>
        /// <param name="filePath">İşlenecek resim dosyasının tam yolu.</param>
        public static OcvResultEntity.OcvResult AnalyzeImage(string filePath)
        {
            try
            {
                // -----------------------------------------------------------------
                // 1️⃣ Resmi bir kez belleğe al (auto‑dispose ile kaynak temizlenir)
                // -----------------------------------------------------------------
                using var src = Cv2.ImRead(filePath, ImreadModes.Color);
                if (src.Empty())
                {
                    Console.WriteLine($"[OCV] Image could not be loaded: {filePath}");
                    return new OcvResultEntity.OcvResult { IsReadable = false };
                }

                // -----------------------------------------------------------------
                // 2️⃣ DataMatrix konumunu bul (varsa)
                // -----------------------------------------------------------------
                var dmRect = DatamatrixFinder.FindDataMatrix(src);
                bool dmRectIsValid = dmRect != default && dmRect.Width > 0 && dmRect.Height > 0;

                // -----------------------------------------------------------------
                // 3️⃣ DataMatrix var ise GS1‑library ile çöz
                // -----------------------------------------------------------------
                DatamatrixEntity? dmEntity = null;
                bool hasDataMatrix = false;

                if (dmRectIsValid)
                {
                    var dmResult = DatamatrixReader.ReadDataMatrix(src, dmRect);
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
                            Man     = dict.GetValueOrDefault("11") ?? string.Empty,
                            ExpDate = dict.GetValueOrDefault("17") ?? string.Empty
                        };
                    }
                }

                // -----------------------------------------------------------------
                // 4️⃣ OCR için görüntüyü işleme (kırpma, gürültü azaltma vb.)
                // -----------------------------------------------------------------
                using var ocrMat = ImageProcessing.ProcessFile(src, dmRect);

                // -----------------------------------------------------------------
                // 5️⃣ OCR (Tesseract vb.) – düz metin elde edilir
                // -----------------------------------------------------------------
                string rawOcrText = Ocr.Read(ocrMat);
                Console.WriteLine(rawOcrText);                 // DEBUG amacıyla, istenirse silinebilir

                // -----------------------------------------------------------------
                // 6️⃣ OCR’dan “DataMatrix‑formatı” (GTIN, SN …) bilgileri çekilir
                // -----------------------------------------------------------------
                var ocrData = BuildOcrDataFromText(rawOcrText);

                // -----------------------------------------------------------------
                // 7️⃣ DataMatrix yoksa ve OCR’da GTIN/SN bulunamadıysa
                //    “Box” (BatchNo / MfgDate / ExpDate / Price) bilgilerini dene
                // -----------------------------------------------------------------
                BoxEntity? box = null;
                if (!hasDataMatrix && (string.IsNullOrWhiteSpace(ocrData.Gtin) || string.IsNullOrWhiteSpace(ocrData.Sn)))
                {
                    box = BuildBoxFromText(rawOcrText);
                }

                // -----------------------------------------------------------------
                // 8️⃣ Sonucun okunabilirliği (IsReadable) belirlenir
                // -----------------------------------------------------------------
                bool isReadable = false;

                if (hasDataMatrix && dmEntity != null)
                {
                    // DataMatrix geçerli ise GTIN ve SN dolu mu?
                    isReadable = !string.IsNullOrWhiteSpace(dmEntity.Gtin) &&
                                 !string.IsNullOrWhiteSpace(dmEntity.Sn);
                }
                else if (!string.IsNullOrWhiteSpace(ocrData.Gtin) && !string.IsNullOrWhiteSpace(ocrData.Sn))
                {
                    // DataMatrix yok, ama OCR’dan GTIN+SN var
                    isReadable = true;
                }
                else if (box != null && (!string.IsNullOrWhiteSpace(box.BatchNo) ||
                                         !string.IsNullOrWhiteSpace(box.ExpDate)))
                {
                    // Sadece Box bilgileri bulunduysa da okunabilir kabul edilebilir
                    isReadable = true;
                }

                // -----------------------------------------------------------------
                // 9️⃣ Tüm bilgileri birleştirerek dönüş yapılır
                // -----------------------------------------------------------------
                return new OcvResultEntity.OcvResult
                {
                    HasDataMatrix = hasDataMatrix,
                    IsReadable    = isReadable,
                    DataMatrix    = dmEntity,
                    Box           = box,
                    RawOcrText    = rawOcrText,
                    OcrData       = ocrData          // OcrData her zaman doldurulur (null olmaz)
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OCV] Unexpected error: {ex.Message}");
                return new OcvResultEntity.OcvResult { IsReadable = false };
            }
        }

        // -----------------------------------------------------------------
        //  Helper: OCR metninden GTIN, SN, LOT, MAN, EXP çekilir
        // -----------------------------------------------------------------
        private static DatamatrixEntity BuildOcrDataFromText(string text)
        {
            var gtinMatch = RegexHelper.Gtin.Match(text);
            var snMatch   = RegexHelper.Sn.Match(text);
            var lotMatch  = RegexHelper.Lot.Match(text);
            var manMatch  = RegexHelper.Man.Match(text);
            var expMatch  = RegexHelper.Exp.Match(text);

            // OCR bazen SN'yi SH olarak okuyabilir → ek bir arama
            if (!snMatch.Success)
            {
                var shRegex = new Regex(@"SH:?\s*([A-Z0-9]+)", RegexOptions.IgnoreCase);
                snMatch = shRegex.Match(text);
            }

            return new DatamatrixEntity
            {
                Gtin    = gtinMatch.Success ? gtinMatch.Groups[1].Value : string.Empty,
                Sn      = snMatch.Success   ? snMatch.Groups[1].Value   : string.Empty,
                Lot     = lotMatch.Success  ? lotMatch.Groups[1].Value  : string.Empty,
                Man     = manMatch.Success  ? manMatch.Groups[1].Value  : string.Empty,
                ExpDate = expMatch.Success  ? expMatch.Groups[1].Value  : string.Empty
            };
        }

        // -----------------------------------------------------------------
        //  Helper: DataMatrix bulunmadığında Box bilgilerini çıkar
        // -----------------------------------------------------------------
        private static BoxEntity? BuildBoxFromText(string text)
        {
            var batchMatch = RegexHelper.BatchNo.Match(text);
            var mfgMatch   = RegexHelper.MfgDate.Match(text);
            var expMatch   = RegexHelper.ExpDate.Match(text);
            var priceMatch = RegexHelper.Price.Match(text);

            // Hiç bir eşleşme yoksa Box nesnesine ihtiyaç yoktur
            if (!batchMatch.Success && !mfgMatch.Success && !expMatch.Success && !priceMatch.Success)
                return null;

            return new BoxEntity
            {
                BatchNo = batchMatch.Success ? batchMatch.Groups[1].Value : string.Empty,
                MfgDate = mfgMatch.Success   ? mfgMatch.Groups[1].Value   : string.Empty,
                ExpDate = expMatch.Success   ? expMatch.Groups[1].Value   : string.Empty,
                Price   = priceMatch.Success ? priceMatch.Groups[1].Value : string.Empty
            };
        }
    }
}
