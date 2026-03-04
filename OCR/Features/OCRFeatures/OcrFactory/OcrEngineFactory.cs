using System.Runtime.InteropServices;

namespace OCR.Features.OCRFeatures;

public static class OcrEngineFactory
{
    public static IOcrEngine CreateEngine(string lang = "eng")
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new WindowsOcrEngine(lang);
        }
        else
        {
            return new MacNativeOcrEngine(lang);
        }
    }
}
