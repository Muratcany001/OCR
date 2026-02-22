using OpenCvSharp;

namespace OCR.Packages;

public class DatamatrixFinder
{
    public static Rect FindDataMatrix(Mat rawSrc)
    {
        // recolor and use threshold for contour finding
        using Mat grayFull = new Mat();
        using Mat threshFull = new Mat();
        Cv2.CvtColor(rawSrc, grayFull, ColorConversionCodes.BGR2GRAY);
        Cv2.Threshold(grayFull, threshFull, 100, 255, ThresholdTypes.BinaryInv);
        
        // find contours in image
        Cv2.FindContours(threshFull, out var contours, out _,
            RetrievalModes.External, ContourApproximationModes.ApproxSimple);
        
        // Datamatrix icin kare secimi yap
        // yukseligi ve genisligi farkinin en dusuk oldugu sekli al.
        Rect qr = default;
        double minDiff = double.MaxValue;
        
        // find sharpest rect on contours
        foreach (var c  in contours)
        {
            var r = Cv2.BoundingRect(c);
            if (r.Width > 50 && r.Height > 50)
            {
                // find rect on shapes
                double diff = Math.Abs(r.Width- r.Height);
                if (diff < minDiff)
                {
                    
                    minDiff = diff;
                    qr = r;
                }
            }
        }
        return qr;
    }
    
}