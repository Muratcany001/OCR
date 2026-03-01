using System.Runtime.InteropServices;
using OCR.Features.OCRFeatures;
using OpenCvSharp;

namespace OCR;

/// <summary>
/// Tesseract OCR motoru ile görselden metin okuma işlemini yöneten sınıf.
/// Görseli geçici dosyaya yazarak Tesseract CLI üzerinden (Mac) veya TesseractEngine üzerinden (Windows) işlem yapar.
/// </summary>
public sealed class CharacterRecognition : IDisposable
{
    private readonly IOcrEngine _engine;

    public CharacterRecognition(string lang = "eng")
    {
        _engine = OcrEngineFactory.CreateEngine(lang);
    }
    
    public string Read(Mat image, string psm = "6")
    {
        return _engine.Read(image, psm);
    }

    public void Dispose()
    {
        _engine.Dispose();
    }
}