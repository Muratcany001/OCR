using OCR.Packages;
using OpenCvSharp;
using ZXing;
using ZXing.Common;
using ZXing.Datamatrix;

namespace OCR.Features.DatamarixFeatures;

/// <summary>
/// ZXing kütüphanesi kullanarak DataMatrix barkodunun içeriğini okuyan sınıf.
/// Dışarıdan verilen Mat ve DataMatrix koordinatları ile çalışır (tekrar okuma yapmaz).
/// </summary>
public class DatamatrixReader
{
    /// <summary>
    /// Verilen görsel ve DataMatrix koordinatları ile barkodu decode eder.
    /// </summary>
    /// <param name="rawSrc">Orijinal kaynak görsel (Ocv'den paylaşılan).</param>
    /// <param name="dmRect">DatamatrixFinder'dan dönen bounding rectangle.</param>
    /// <returns>DataMatrix içeriği. Okunamazsa string.Empty döner.</returns>
    public static string ReadDataMatrix(Mat rawSrc, Rect dmRect)
    {
        if (dmRect.Width <= 0 || dmRect.Height <= 0)
        {
            Console.WriteLine("Datamatrix not found...");
            return string.Empty;
        }
        
        // DataMatrix çevresine padding ekle
        int padding = 20;
        int x = Math.Max(0, dmRect.X - padding);
        int y = Math.Max(0, dmRect.Y - padding);
        int w = Math.Min(rawSrc.Width - x, dmRect.Width + padding * 2);
        int h = Math.Min(rawSrc.Height - y, dmRect.Height + padding * 2);
        
        // DataMatrix bölgesini kırp
        using Mat dm = rawSrc[new Rect(x, y, w, h)];
        using Mat gray = new Mat();
        Cv2.CvtColor(dm, gray, ColorConversionCodes.BGR2GRAY);
        
        int width = gray.Width;
        int height = gray.Height;

        byte[] pixels = new byte[width * height];
        gray.GetArray(out pixels);
        
        var luminanceSource = new RGBLuminanceSource(
            pixels, width, height,
            RGBLuminanceSource.BitmapFormat.Gray8
        );
        
        var binarizer = new GlobalHistogramBinarizer(luminanceSource);
        var binaryBitmap = new BinaryBitmap(binarizer);
        var reader = new DataMatrixReader();
        var result = reader.decode(binaryBitmap);
        
        if (result == null)
        {
            Console.WriteLine("Datamatrix not found...");
            return string.Empty;
        }
        return result.Text ?? string.Empty;
    }
}