
using OpenCvSharp;
using Tesseract;

namespace OCR;

public sealed class CharacterRecognition : IDisposable
{
    private readonly TesseractEngine _engine;

    public CharacterRecognition(string lang = "eng")
    {
        // tessdata klasörünü proje kökünde oluştur
        string tessDataPath = @"C:\Program Files\tessdata";  // ← KLASÖR YOLU


        if (!Directory.Exists(tessDataPath))
        {
            throw new DirectoryNotFoundException($"tessdata folder not found at: {tessDataPath}");
        }

        _engine = new TesseractEngine(tessDataPath, lang, EngineMode);
    }

    public string Read(Mat image)
    {
        if (image == null || image.Empty())
            return string.Empty;

        try
        {
            // Mat -> MemoryStream
            using var memoryStream = new MemoryStream();
            Cv2.ImEncode(".bmp", image, out var imageData);
            memoryStream.Write(imageData, 0, imageData.Length);
            memoryStream.Position = 0;

            // MemoryStream -> Pix
            using var pix = Pix.LoadFromMemory(memoryStream.ToArray());

            // OCR yap - PARAMETRESİZ Process
            using var page = _engine.Process(pix,pageSegMode: PageSegMode.SingleBlock, ocrEngineMode: OcrEngineMode.Default);

            return page.GetText()?.Trim() ?? string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"OCR Error: {ex.Message}");
            return string.Empty;
        }
    }

    public void Dispose() => _engine?.Dispose();
}
