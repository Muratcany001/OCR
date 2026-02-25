    using OCR.Packages;
    using OpenCvSharp;

    namespace Features.OCRFeatures.ImageProcessing;

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
            int roiX = qr.X + qr.Width ;
            int roiY = Math.Max(0, qr.Y - 40);
            int roiW = Math.Min(src.Width - roiX - 10, 630);
            int roiH = Math.Min(qr.Height + 200, src.Height - roiY);
        
            // datamatrix bulunmazsa sabit roi kullan
            if (roiX >= src.Width || roiW <= 0 || roiH <= 0)
            {
                Console.WriteLine("ROI geçersiz, sabit ROI kullanılıyor");
                roiX = 130;
                roiY = 330;
                roiW = 630;
                roiH = 335;
            }
            roiX = Math.Clamp(roiX, 0, src.Width - 1);
            roiY = Math.Clamp(roiY, 0, src.Height - 1);
            roiW = Math.Clamp(roiW, 1, src.Width - roiX);
            roiH = Math.Clamp(roiH, 1, src.Height - roiY);
            Mat cropped = src[new Rect(roiX, roiY, roiW, roiH)];
            
            Cv2.ImShow("123", cropped);
            Cv2.WaitKey();
            // Grayscale + Adaptive Threshold
            using Mat gray = new Mat();
            Mat binary = new Mat();
            Cv2.CvtColor(cropped, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.GaussianBlur(gray, gray, new Size(3,3), 0);
            
            Cv2.AdaptiveThreshold(gray, binary, 255,
                AdaptiveThresholdTypes.GaussianC,
                ThresholdTypes.Binary, 31, 7);
            
             var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(2,2));
             Cv2.MorphologyEx(binary, binary, MorphTypes.Close, kernel);
             
            src.Dispose();
           
            return binary;
        }
    }