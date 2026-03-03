
using OCR.Entities;

namespace OCR.Features.OCVFeatures;

public static class Validator
{
    public static int ResultValidator(OcvResultEntity.OcvResult ocvResult)
    {
        string target = ocvResult.DatamatrixOutput ?? "";
        string ocr = ocvResult.DatamatrixOcrOutput ?? "";
        
        if (string.IsNullOrEmpty(target)) 
        {
            return 0;
        }
        int ocrScore = 0;
        for (int i = 0; i < target.Length; i++)
        {
            if (i < ocr.Length)
            {
                if (ocr[i] == target[i])
                {
                    ocrScore++;
                }
            }
        }
        if (target.Length == 0) return 0;
        return (ocrScore * 100) / target.Length;
    }
}