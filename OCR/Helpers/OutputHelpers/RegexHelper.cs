using System.Text.RegularExpressions;

namespace OCR.Helpers.OutputHelpers;

/// <summary>
/// OCR çıktısını parse etmek için kullanılan compiled Regex pattern'leri.
/// Static readonly olarak tanımlanır, her çağrıda yeniden derlenmez.
/// </summary>
public static class RegexHelper
{
    // DataMatrix etiket pattern'leri (label ile)
    public static readonly Regex Gtin = new(@"GTIN:?\s*(\d{14})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public static readonly Regex Sn = new(@"S[NM]:?\s*([A-Z0-9]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public static readonly Regex Lot = new(@"LOT:?\s*([A-Z0-9]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public static readonly Regex Man = new(@"MAN:?\b(\d{2})[\/7]?20(\d{2})\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public static readonly Regex Exp = new(@"EXP:?\s*(\d{2}).*?(\d{2})\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    // Batch No: Harflerle başlar, sayıyla biter (örn: VTTO1, VIT01, ABC123)
    public static readonly Regex BatchNo = new(@"^([A-Z]{2,5}\d{1,4})$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
    
    // Tarih formatları: MM/YYYY
    public static readonly Regex DoubleDate =
        new(@"EXP:?\s*(\d{4})",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    // Price: PRICE kelimesinden sonra gelen sayılar
    // public static readonly Regex Price = new(@"Price\s*:?\s*([0-9.,]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public static readonly Regex Price = new(@"Price\s*:?\s*([0-9.,]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
}