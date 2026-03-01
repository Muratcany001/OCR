using OpenCvSharp;

namespace OCR.Features.OCRFeatures;

public interface IOcrEngine : IDisposable
{
    string Read(Mat image, string psm = "6");
}
