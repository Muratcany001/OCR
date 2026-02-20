using System.Diagnostics;
using OCR.Packages;
using OpenCvSharp;

namespace OCR;

public class ImageProcessing
{
    public static Mat ProcessFile(string filePath)
    {

        Mat src = Cv2.ImRead(filePath);

        if (src.Empty())
        {
            Console.WriteLine("Image could not be loaded");
            return null;
        }
        // data matrix rect finder
        var qr= DatamatrixFinder.FindDataMatrix(src);
        
        // Data Matrix'in sagini ROI olarak al
        int roiX = qr.X + qr.Width + 20;
        int roiY = Math.Max(0, qr.Y - 40);
        int roiW = Math.Min(src.Width - roiX - 10, 630);
        int roiH = Math.Min(qr.Height + 200, src.Height - roiY);
        
        // datamatrix bulunmazsa sabit roi kullan
        if (roiX >= src.Width || roiW <= 0 || roiH <= 0)
        {
            Console.WriteLine("ROI geçersiz, sabit ROI kullanılıyor");
            roiX = 160;
            roiY = 360;
            roiW = 630;
            roiH = 375;
        }
        Mat cropped = src[new Rect(roiX, roiY, roiW, roiH)];
        
        // Grayscale + Adaptive Threshold
        using Mat gray = new Mat();
        Mat binary = new Mat();
        Cv2.CvtColor(cropped, gray, ColorConversionCodes.BGR2GRAY);
        Cv2.AdaptiveThreshold(gray, binary, 255,
            AdaptiveThresholdTypes.GaussianC,
            ThresholdTypes.Binary, 21, 6);
        
        Cv2.ImShow("Image", binary);
        Cv2.WaitKey();
        src.Dispose();
        return binary;
    }
}