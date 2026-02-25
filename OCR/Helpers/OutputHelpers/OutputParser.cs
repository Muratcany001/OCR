using BenchmarkDotNet.Columns;
using OCR.Entities;

namespace OCR.Helpers.OutputHelpers;

/// <summary>
/// OCR çıktısını sabit pozisyon (Substring) ile ayrıştıran parser sınıfı.
/// DataMatrix ve kutu etiketi senaryoları için ayrı metotlar sağlar.
/// </summary>
public class OutputParser
{
    /// <summary>
    /// DataMatrix içeren görsellerin OCR çıktısını parse eder.
    /// Beklenen format: GTIN(14) + SN(10) + LOT(8) + MAN(6) = toplam 38 karakter.
    /// </summary>
    /// <param name="text">Birleştirilmiş OCR metin çıktısı (minimum 38 karakter).</param>
    /// <returns>Parse edilmiş DatamatrixEntity nesnesi.</returns>
    public static DatamatrixEntity DatamatrixParse(string text)
    {
        var parsed = new DatamatrixEntity
        {
            Gtin = text.Substring(0, 14),
            Sn   = text.Substring(14, 10),
            Lot  = text.Substring(24, 8),
            Man  = text.Substring(32, 6)
        };
        return parsed;
    }
    /// <summary>
    /// Kutu etiketi (DataMatrix olmayan) görsellerin OCR çıktısını parse eder.
    /// Beklenen format: BatchNo(5) + MfgDate(6) + ExpDate(6) = toplam 17 karakter.
    /// </summary>
    /// <param name="text">Birleştirilmiş OCR metin çıktısı (minimum 17 karakter).</param>
    /// <returns>Parse edilmiş BoxEntity nesnesi.</returns>
    public static BoxEntity OCRParse(string text)
    {
        var parsed = new BoxEntity()
        {
            BatchNo = text.Substring(0, 5),
            MfgDate   = text.Substring(5, 6),
            ExpDate  = text.Substring(11, 6),
            // Price  = text.Substring(17, 5)
        };
        return parsed;
    }
}