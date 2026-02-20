using OpenCvSharp;

namespace OCR.Packages;

public class DatamatrixFinder
{
    public static Rect FindDataMatrix(Mat src)
    {
        Mat grayFull = new Mat();
        Mat threshFull = new Mat();
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

    public static Mat GetImageMatrix(Mat src)
    {
        var rects = FindDataMatrix(src);
        Mat dm = src[new Rect(rects.Left - 20, rects.Top - 20, rects.Width + 40, rects.Height + 40)];
        Mat dmBinary = new Mat();
        Cv2.CvtColor(dm, dmBinary, ColorConversionCodes.BGR2GRAY);
        return dmBinary;
    }
}