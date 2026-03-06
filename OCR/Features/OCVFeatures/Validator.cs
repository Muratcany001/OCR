
using OCR.Entities;

namespace OCR.Features.OCVFeatures;

public static class Validator
{
    public static int ResultValidator(OcvResultEntity.OcvResult ocvResult)
    {
        string target = ocvResult.DatamatrixOutput ?? "";
        string ocr = ocvResult.DatamatrixOcrOutput ?? "";

        if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(ocr)) return 0;

        int distance = ComputeLevenshteinDistance(target, ocr);
        
        // Benzerlik skoru formülü: (Maksimum Uzunluk - Hata Sayısı) / Maksimum Uzunluk
        int maxLength = Math.Max(target.Length, ocr.Length);
        if (maxLength == 0) return 0;

        double similarity = (double)(maxLength - distance) / maxLength;
        return (int)(similarity * 100);
    }

    private static int ComputeLevenshteinDistance(string s, string t)
    {
        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        if (n == 0) return m;
        if (m == 0) return n;

        for (int i = 0; i <= n; d[i, 0] = i++) ;
        for (int j = 0; j <= m; d[0, j] = j++) ;

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }
        return d[n, m];
    }
}