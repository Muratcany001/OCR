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
            int roiH = Math.Min(dmRect.Height + 250, src.Height - roiY);
            
            // DataMatrix bulunmazsa sabit ROI kullan
            if (dmRect.Width <= 0 || dmRect.Height <= 0 || dmRect.Width == src.Width)
            {
                Console.WriteLine("ROI geçersiz, sabit ROI kullanılıyor");
                roiX = 470;
                roiY = 440;
                roiW = 700;
                roiH = 250;
            }
            roiX = Math.Clamp(roiX, 0, src.Width - 1);
            roiY = Math.Clamp(roiY, 0, src.Height - 1);
            roiW = Math.Clamp(roiW, 1, src.Width - roiX);
            roiH = Math.Clamp(roiH, 1, src.Height - roiY);
            
            // Debug display is commented out to save CPU/Memory
            // using Mat debug = src.Clone();
            // Cv2.Rectangle(debug, new Rect(roiX, roiY, roiW, roiH), Scalar.Red, 3);
            // Cv2.ImShow("ROI Debug", debug);
            // Cv2.WaitKey();
            
            using Mat cropped = src[new Rect(roiX, roiY, roiW, roiH)];
            
            Cv2.Resize(cropped, cropped, new Size(), 1.25, 1.25, InterpolationFlags.Lanczos4);
    
            using Mat gray = new Mat();
            Mat binary = new Mat();
            Cv2.CvtColor(cropped, gray, ColorConversionCodes.BGR2GRAY);
            
            Cv2.GaussianBlur(gray, gray, new Size(3,3), 0);
            
            Cv2.AdaptiveThreshold(gray, binary, 255,
                AdaptiveThresholdTypes.GaussianC,
                ThresholdTypes.Binary, 31, 10);
            
            using var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(2,2));
            Cv2.MorphologyEx(binary, binary, MorphTypes.Close, kernel);
            
            // Cv2.Erode(binary, binary, kernel); 
            // Cv2.ImShow("123",binary);
            // Cv2.WaitKey();
            return binary;
        }
    }