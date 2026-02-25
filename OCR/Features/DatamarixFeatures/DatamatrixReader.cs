using OCR.Packages;
using OpenCvSharp;
using ZXing;
using ZXing.Common;
using ZXing.Datamatrix;

namespace OCR.Features.DatamarixFeatures;

/// <summary>
/// ZXing kütüphanesi kullanarak DataMatrix barkodunun içeriğini okuyan sınıf.
/// DatamatrixFinder ile bulunan bölgeyi crop ederek decode işlemi yapar.
/// </summary>
public class DatamatrixReader
{
    /// <summary>
    /// Görseldeki DataMatrix barkodunu bulur ve GS1 formatındaki içeriğini okur.
    /// </summary>
    /// <param name="filePath">Okunacak görselin dosya yolu.</param>
    /// <returns>DataMatrix içeriği. Okunamazsa string.Empty döner.</returns>
    public static string ReadDataMatrix(string filePath)
    {
        //input src and find datamatrix
        using var rawSrc = Cv2.ImRead(filePath);
        var dmCordinates = DatamatrixFinder.FindDataMatrix(rawSrc);
        var r = dmCordinates;
        
        // find borders of dm coordinates
        int padding = 20;
        int x = Math.Max(0, r.X - padding);
        int y = Math.Max(0, r.Y - padding);
        int w = Math.Min(rawSrc.Width - x, r.Width + padding * 2);
        int h = Math.Min(rawSrc.Height - y, r.Height + padding * 2);
        
        // crop datamatrix from src
        using Mat dm = rawSrc[new Rect(x,y,w,h)];
        using Mat gray = new Mat();
        // recolor for reader
        Cv2.CvtColor(dm, gray, ColorConversionCodes.BGR2GRAY);
        
        int width = gray.Width;
        int height = gray.Height;

        byte[] pixels = new byte[width * height];
        gray.GetArray(out pixels);
        // wrap dm object   with luminance source for use zxing library
        var luminanceSource = new RGBLuminanceSource(
            pixels,
            width,
            height,
            RGBLuminanceSource.BitmapFormat.Gray8
        );
        // do black-white filter with globalhistogrambinarizer  
        var binarizer = new GlobalHistogramBinarizer(luminanceSource);
        var binaryBitmap = new BinaryBitmap(binarizer);
        var reader = new DataMatrixReader();
        var result = reader.decode(binaryBitmap);
        if (result == null)
        {
            Console.WriteLine("Datamatrix reader failed");
            return string.Empty;
        }
        return result.Text ??  string.Empty;
    }
}