    using OCR.Packages;
    using OpenCvSharp;

    namespace Features.OCRFeatures.ImageProcessing;

    /// <summary>
    /// Görseli OCR için hazırlayan sınıf.
    /// Paylaşılan Mat ve DataMatrix koordinatlarına göre ROI belirler.
    /// </summary>
    public class ImageProcessing
    {
        /// <summary>
        /// Paylaşılan görseli OCR için hazırlar. Tekrar diskten okuma yapmaz.
        /// </summary>
        /// <param name="src">Ocv'den paylaşılan orijinal görsel.</param>
        /// <param name="dmRect">DatamatrixFinder sonucu. default ise sabit ROI kullanılır.</param>
        /// <returns>Preprocessing uygulanmış binary görsel.</returns>
        public static Mat ProcessFile(Mat src, Rect dmRect)
        {
            // Data Matrix'in sagini ROI olarak al
            int roiX = dmRect.X + dmRect.Width;
            int roiY = Math.Max(0, dmRect.Y - 40);
            int roiW = Math.Min(src.Width - roiX - 10, 630);
            int roiH = Math.Min(dmRect.Height + 200, src.Height - roiY);
        
            // DataMatrix bulunmazsa sabit ROI kullan
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
            
            cropped.Dispose();
            Cv2.Resize(binary, binary, new Size(), 2.0 ,2.0, InterpolationFlags.Area );
            // Cv2.ImShow("123",binary);
            // Cv2.WaitKey();
            return binary;
        }
    }