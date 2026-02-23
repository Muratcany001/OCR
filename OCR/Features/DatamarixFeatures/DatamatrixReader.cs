using System.Drawing;
using OCR.Packages;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using ZXing;
using ZXing.Common;
using ZXing.Datamatrix;
using ZXing.Windows.Compatibility;

namespace OCR;

public class DatamatrixReader
{
    
    public static string ReadDataMatrix(string filePath)
    {
        
        using var rawSrc = Cv2.ImRead(filePath);
        var dmCordinates = DatamatrixFinder.FindDataMatrix(rawSrc);
        var r = dmCordinates;
        int padding = 20;
        int x = Math.Max(0, r.X - padding);
        int y = Math.Max(0, r.Y - padding);
        int w = Math.Min(rawSrc.Width - x, r.Width + padding * 2);
        int h = Math.Min(rawSrc.Height - y, r.Height + padding * 2);
        
        using Mat dm = rawSrc[new Rect(x,y,w,h)];
        Cv2.ImShow("Datamatrix", dm);
        Cv2.WaitKey();
        using Mat gray = new Mat();
        Cv2.CvtColor(dm, gray, ColorConversionCodes.BGR2GRAY);
        
        int width = gray.Width;
        int height = gray.Height;

        byte[] pixels = new byte[width * height];
        gray.GetArray(out pixels);

        var luminanceSource = new RGBLuminanceSource(
            pixels,
            width,
            height,
            RGBLuminanceSource.BitmapFormat.Gray8
        );

        var binarizer = new GlobalHistogramBinarizer(luminanceSource);
        var binaryBitmap = new BinaryBitmap(binarizer);
        var reader = new DataMatrixReader();
        var result = reader.decode(binaryBitmap);
        
        return result?.Text;
    }
}