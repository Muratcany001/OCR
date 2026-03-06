using OpenCvSharp;

namespace OCR.Helpers.DatamatrixHelpers;

/// <summary>
/// Görsel içinde DataMatrix barkodunun konumunu tespit eden sınıf.
/// Contour analizi ile kareye en yakın şekli DataMatrix olarak tanımlar.
/// </summary>
public class DatamatrixFinder
{
    /// <summary>
    /// Görselde contour analizi yaparak DataMatrix barkodunun bounding rectangle'ını bulur.
    /// Yükseklik ve genişlik farkı en az olan dikdörtgen DataMatrix olarak kabul edilir.
    /// </summary>
    /// <param name="rawSrc">Orijinal BGR formatında kaynak görsel.</param>
    /// <returns>DataMatrix'in bounding rectangle'ı. Bulunamazsa default (0,0,0,0) döner.</returns>
    public static Rect FindDataMatrix(Mat rawSrc)
    {
        // recolor and use threshold for contour finding
        using Mat grayFull = new Mat();
        using Mat threshFull = new Mat();
        if (rawSrc.Channels() == 1)
            rawSrc.CopyTo(grayFull);
        else
            Cv2.CvtColor(rawSrc, grayFull, ColorConversionCodes.BGR2GRAY);
        Cv2.Threshold(grayFull, threshFull, 0, 255, ThresholdTypes.BinaryInv | ThresholdTypes.Otsu);
        
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
            if (r.Width > 50 && r.Height > 50 && r.Width < 600 && r.Height < 600)
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