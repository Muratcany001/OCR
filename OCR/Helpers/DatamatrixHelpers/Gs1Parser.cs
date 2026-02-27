namespace OCR.Helpers.DatamatrixHelpers;

/// <summary>
/// GS1 barkod verisini Application Identifier (AI) bazında parse eden sınıf.
/// Sabit ve değişken uzunluklu AI'ları destekler (GTIN, Lot, Serial No, Expiry Date vb.).
/// </summary>
public class Gs1Parser
{
    private static readonly Dictionary<string, (string Name, int FixedLength)> KnownAIs = new()
    {
        { "00", ("SSCC", 18) },
        { "01", ("GTIN", 14) },
        { "02", ("GTIN of contained", 14) },
        { "10", ("Lot/Batch No", 0) },       // variable, max 20
        { "11", ("Production Date", 6) },
        { "17", ("Expiry Date", 6) },
        { "21", ("Serial No", 0) },           // variable, max 20
        { "30", ("Quantity", 0) },
        { "310", ("Net Weight kg", 6) },
        { "37", ("Count", 0) },
        { "400", ("Order Number", 0) },
        { "420", ("Ship To Postal", 0) },
    };

    private const char FNC1 = '\x1D'; // Group Separator

    /// <summary>
    /// GS1 formatındaki barkod verisini AI-değer çiftlerine ayrıştırır.
    /// FNC1 (Group Separator) karakterini ayırıcı olarak kullanır.
    /// </summary>
    /// <param name="data">GS1 formatında ham barkod verisi.</param>
    /// <returns>AI kodu, AI adı ve değerden oluşan tuple listesi.</returns>
    public static List<(string AI, string Name, string Value)> Parse(string data)
    {
        var result = new List<(string AI, string Name, string Value)>();
        int i = 0;

        while (i < data.Length)
        {
            // FNC1 karakterini atla
            if (data[i] == FNC1)
            {
                i++;
                continue;
            }

            // AI'yi bul (2, 3 veya 4 haneli olabilir)
            string ai = null;
            int fixedLen = 0;
            string aiName = "Unknown";

            foreach (int aiLen in new[] { 4, 3, 2 })
            {
                if (i + aiLen > data.Length) continue;

                string candidate = data.Substring(i, aiLen);
                if (KnownAIs.TryGetValue(candidate, out var info))
                {
                    ai = candidate;
                    aiName = info.Name;
                    fixedLen = info.FixedLength;
                    break;
                }
            }

            if (ai == null)
            {
                Console.WriteLine($"Bilinmeyen AI pozisyon {i}: {data.Substring(i)}");
                break;
            }

            i += ai.Length;

            // Değeri oku
            string value;
            if (fixedLen > 0)
            {
                // Sabit uzunluklu
                value = data.Substring(i, fixedLen);
                i += fixedLen;
            }
            else
            {
                // Değişken uzunluklu: FNC1 veya string sonu kadar oku
                int end = data.IndexOf(FNC1, i);
                value = end == -1 ? data.Substring(i) : data.Substring(i, end - i);
                i += value.Length;
            }
            value = value.Replace("/", "");
            result.Add((ai, aiName, value));
        }
        
        return result;
    }
}