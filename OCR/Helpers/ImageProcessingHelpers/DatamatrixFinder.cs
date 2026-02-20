using OpenCvSharp;

namespace OCR.Packages;

public class DatamatrixFinder
{
    public static Rect FindDataMatrix(Mat src)
    {
        using Mat grayFull = new Mat();
        using Mat threshFull = new Mat();
        Cv2.CvtColor(src, grayFull, ColorConversionCodes.BGR2GRAY);
        Cv2.Threshold(grayFull, threshFull, 100, 255, ThresholdTypes.BinaryInv);
        
        Cv2.FindContours(threshFull, out var contours, out _,
            RetrievalModes.External, ContourApproximationModes.ApproxSimple);
        
        // Datamatrix icin kare secimi yap
        var qr = contours
            .Select(c => Cv2.BoundingRect(c))
            .Where(r => r.Width > 50 && r.Height > 50)
            .OrderBy(r => Math.Abs(r.Width - r.Height)) // 
            .FirstOrDefault();
        return qr;
    }
    
}