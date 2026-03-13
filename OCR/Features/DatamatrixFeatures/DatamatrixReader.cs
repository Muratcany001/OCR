using System.Collections.Generic;
using System.Diagnostics;
using OCR.Packages;
using OpenCvSharp;
using ZXing;
using ZXing.Common;
using ZXing.Datamatrix;

namespace OCR.Features.DatamatrixFeatures;

/// <summary>
/// ZXing kütüphanesi kullanarak DataMatrix barkodunun içeriğini okuyan sınıf.
/// Dışarıdan verilen Mat ve DataMatrix koordinatları ile çalışır (tekrar okuma yapmaz).
/// </summary>
public class DatamatrixReader
{
    public string ReadDataMatrix(Mat rawSrc, Rect dmRect)
    {
        if (dmRect.Width <= 0 || dmRect.Height <= 0)
        {
            Console.WriteLine("[DM] Rect geçersiz — DatamatrixFinder hiçbir kare bulamadı.");
            return string.Empty;
        }
        
        Stopwatch sw = Stopwatch.StartNew();
        
        int padding = 20;
        int x = Math.Max(0, dmRect.X - padding);
        int y = Math.Max(0, dmRect.Y - padding);    
        int w = Math.Min(rawSrc.Width  - x, dmRect.Width  + padding * 2);
        int h = Math.Min(rawSrc.Height - y, dmRect.Height + padding * 2);

        using Mat dm   = rawSrc[new Rect(x, y, w, h)];
        
        using Mat gray = new Mat();
        Cv2.CvtColor(dm, gray, ColorConversionCodes.BGR2GRAY);
        
        using Mat upscaled = new Mat();
        Cv2.Resize(gray, upscaled, new Size(), 2, 2, InterpolationFlags.Linear);
        // Cv2.ImShow("123",upscaled);
        // Cv2.WaitKey();
        var hints = new Dictionary<DecodeHintType, object>
        {
            { DecodeHintType.TRY_HARDER, true },
            { DecodeHintType.CHARACTER_SET, "UTF-8" },
            { DecodeHintType.POSSIBLE_FORMATS, new List<BarcodeFormat> { BarcodeFormat.DATA_MATRIX } }
        };
        var result = TryDecode(upscaled, hints);
        

        if (result == null)
        {
            Console.WriteLine("Decode edilemedi.");
            return string.Empty;
        }

        sw.Stop();
        Console.WriteLine($"[DM] {sw.ElapsedMilliseconds} ms — decode başarılı.");
        return result.Text ?? string.Empty;
    }

    private static Result? TryDecode(Mat gray, Dictionary<DecodeHintType, object> hints)
    {
        if (gray.Empty()) return null;

        // 1. ADIM: Keskinleştirme (Unsharp Mask)
        // Bulanık (blur) resimlerde modül kenarlarını belirginleştirir.
        using Mat sharpened = new Mat();
        using Mat blur = new Mat();
        Cv2.GaussianBlur(gray, blur, new Size(0, 0), 3);
        Cv2.AddWeighted(gray, 1.5, blur, -0.5, 0, sharpened);

        // 2. ADIM: Binarization (Otsu Threshold)
        // Gri tonları net siyah ve beyaza çevirir. ZXing buna bayılır.
        using Mat binary = new Mat();
        Cv2.Threshold(sharpened, binary, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);

        // 3. ADIM: İnce Ayar (Morphology)
        // Eğer modüller birbirine akmışsa (senin paylaştığın resimlerdeki gibi), 
        // pikselleri çok hafif daraltarak (Erode) modülleri ayırabiliriz.
        using Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(2, 2));
        Cv2.MorphologyEx(binary, binary, MorphTypes.Open, kernel);

        // 4. ADIM: Veriyi Hazırla
        byte[] pixels;
        binary.GetArray(out pixels);
    
        var luminance = new RGBLuminanceSource(
            pixels, 
            binary.Width, 
            binary.Height, 
            RGBLuminanceSource.BitmapFormat.Gray8);

        var reader = new DataMatrixReader();

        // Önce Hybrid sonra Global binarizer dene (ZXing stratejisi)
        var result = reader.decode(new BinaryBitmap(new HybridBinarizer(luminance)), hints);
        return result ?? reader.decode(new BinaryBitmap(new GlobalHistogramBinarizer(luminance)), hints);
    }
}