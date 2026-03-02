
using System.Diagnostics;
using OCR.Features.OCVFeatures;
using OpenCvSharp;

namespace OCR.Packages;

public class WarmupHelper
{
    public static void Warmup()
    {
        Console.WriteLine("Warming up...");
        Stopwatch stopwatch = Stopwatch.StartNew();
        using Mat warmupMat = new Mat(200,200,MatType.CV_8UC3 , new Scalar(255));
        Ocv.Ocr.Read(warmupMat);
        stopwatch.Stop();
        Console.WriteLine("Warmup time "+stopwatch.ElapsedMilliseconds);
    }
}