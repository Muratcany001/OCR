using BenchmarkDotNet.Columns;
using OCR.Entities;

namespace OCR.Helpers.OutputHelpers;

public class OutputParser
{
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