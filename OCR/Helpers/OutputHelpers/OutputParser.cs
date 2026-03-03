using OCR.Entities;

namespace OCR.Helpers.OutputHelpers;

/// <summary>
/// Entity'leri tek string'e dönüştüren ve string'den entity'ye geri parse eden helper sınıfı.
/// </summary>
public class OutputParser
{
    // === Entity → String (Birleştirme) ===
    
    /// <summary>
    /// DatamatrixEntity'yi tek string'e birleştirir.
    /// Format: LOT + GTIN + SN + MAN + EXP
    /// </summary>
    public static string ToStringOutput(DatamatrixEntity entity)
    {
        return $"{entity.Lot}{entity.Gtin}{entity.Sn}{entity.ExpDate}"
            .Replace("/", "");
    }
    
    /// <summary>
    /// BoxEntity'yi tek string'e birleştirir.
    /// Format: BatchNo + MfgDate + ExpDate
    /// </summary>
    public static string ToStringOutput(BoxEntity entity)
    {
        return $"{entity.BatchNo}{entity.MfgDate}{entity.ExpDate}"
            .Replace("/", "");
    }
    
    // === String → Entity (Ayrıştırma) ===
    
    /// <summary>
    /// Birleştirilmiş string'i DatamatrixEntity'ye parse eder.
    /// Beklenen format: LOT(5) + GTIN(14) + SN(14) + MAN(6) + EXP(6) = 45 karakter.
    /// </summary>
    public static DatamatrixEntity? DatamatrixParse(string text)
    {
        if (string.IsNullOrEmpty(text) || text.Length < 45)
        {
            Console.WriteLine($"Uyarı: Metin kısa ({text?.Length ?? 0}/45). Metin: {text}");
            return null;
        }
        
        return new DatamatrixEntity
        {
            Lot     = text.Substring(0, 5),
            Gtin    = text.Substring(5, 14),
            Sn      = text.Substring(19, 14),
            ExpDate = text.Substring(33, 6)
        };
    }
    
    /// <summary>
    /// Birleştirilmiş string'i BoxEntity'ye parse eder.
    /// Beklenen format: BatchNo(5) + MfgDate(6) + ExpDate(6) = 17 karakter.
    /// </summary>
    public static BoxEntity? OCRParse(string text)
    {
        if (string.IsNullOrEmpty(text) || text.Length < 17)
        {
            Console.WriteLine($"Uyarı: Metin kısa ({text?.Length ?? 0}/17). Metin: {text}");
            return null;
        }
        
        return new BoxEntity
        {
            BatchNo = text.Substring(0, 5),
            MfgDate = text.Substring(5, 6),
            ExpDate = text.Substring(11, 6),
        };
    }
}