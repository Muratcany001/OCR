using OpenCvSharp;
using Tesseract;

namespace OCR.Features.OCRFeatures;

public sealed class WindowsOcrEngine : IOcrEngine
{
    private readonly TesseractEngine _engine;

    public WindowsOcrEngine(string lang)
    {
        string tessDataPath = @"C:\Program Files\tessdata";

        if (!Directory.Exists(tessDataPath))
        {
            throw new DirectoryNotFoundException($"tessdata folder not found at: {tessDataPath}");
        }

        _engine = new TesseractEngine(tessDataPath, lang, EngineMode.Default);
    }

    public string Read(Mat image, string psm = "6")
    {
        if (image == null || image.Empty())
            return string.Empty;

        try
        {
            using var memoryStream = new MemoryStream();
            Cv2.ImEncode(".bmp", image, out var imageData);
            using var pix = Pix.LoadFromMemory(imageData);
            pix.XRes = 300;
            pix.YRes = 300;
            _engine.SetVariable("tessedit_char_whitelist", "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz./:-");
            
            using var page = _engine.Process(pix, pageSegMode: PageSegMode.SingleBlock);

            return page.GetText()?.Trim() ?? string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"OCR Error: {ex.Message}");
            return string.Empty;
        }
    }

    public void Dispose()
    {
        _engine?.Dispose();
    }
}
