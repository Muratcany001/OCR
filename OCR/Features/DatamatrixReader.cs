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
        using Mat dm = rawSrc[dmCordinates];
        
        using Mat gray = new Mat();
        using Mat binary = new Mat();
        Cv2.CvtColor(dm, gray, ColorConversionCodes.BGR2GRAY);
        Cv2.AdaptiveThreshold(gray, binary, 255,
            AdaptiveThresholdTypes.GaussianC,
            ThresholdTypes.Binary, 21, 6);

        int width = binary.Width;
        int height = binary.Height;

        byte[] pixels = new byte[width * height];
        binary.GetArray(out pixels);

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