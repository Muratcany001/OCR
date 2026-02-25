using BenchmarkDotNet.Columns;
using OCR.Entities;

namespace OCR.Helpers.OutputHelpers;

public class OutputParser
{
    public static DatamatrixEntity Parse(string text)
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
}