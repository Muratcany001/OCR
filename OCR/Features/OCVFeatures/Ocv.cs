using OpenCvSharp;

namespace OCR.Features.OCVFeatures;

public class Ocv
{
    private static readonly CharacterRecognition Ocr = new();
    public static void OcvComprasion(string filePath)
    {
        var rawSrc = Cv2.ImRead(filePath);
        var ocr = Ocr.Read(rawSrc);
        
        var rawDm = DatamatrixReader.ReadDataMatrix(filePath);
        
        
    }
}