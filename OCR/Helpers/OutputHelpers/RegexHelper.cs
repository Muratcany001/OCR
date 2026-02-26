using System.Text.RegularExpressions;

namespace OCR.Helpers.OutputHelpers;

/// <summary>
/// OCR çıktısını parse etmek için kullanılan compiled Regex pattern'leri.
/// Static readonly olarak tanımlanır, her çağrıda yeniden derlenmez.
/// </summary>
public static class RegexHelper
{
    // DataMatrix etiket pattern'leri
    public static readonly Regex Gtin = new(@"GTIN:\s*(\d{14})", RegexOptions.Compiled);
    public static readonly Regex Sn = new(@"SN:\s*([A-Za-z0-9]+)", RegexOptions.Compiled);
    public static readonly Regex Lot = new(@"LOT:\s*([A-Za-z0-9]+)", RegexOptions.Compiled);
    public static readonly Regex Man = new(@"MAN:\s*(\d{2}/\d{4})", RegexOptions.Compiled);
    public static readonly Regex Exp = new(@"EXP:\s*\(?(\d{2}/\d{4})\)?", RegexOptions.Compiled);
    
    // Box etiket pattern'leri
    public static readonly Regex BatchNo = new(@"Batch\s*:\s*([A-Za-z0-9]+)", RegexOptions.Compiled);
    public static readonly Regex MfgDate = new(@"Mfg.Date:\s*(\d{2}/\d{4})", RegexOptions.Compiled);
    public static readonly Regex ExpDate = new(@"EXP\.Date:\s*\(?(\d{2}/\d{4})\)?", RegexOptions.Compiled);
}