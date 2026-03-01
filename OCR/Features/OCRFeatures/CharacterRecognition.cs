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

    /// <summary>
    /// Verilen Mat görselini Tesseract ile okuyarak metin döndürür.
    /// İşletim sistemine göre CLI veya kütüphane kullanır.
    /// </summary>
    /// <param name="image">OCR uygulanacak işlenmiş görsel (Mat).</param>
    /// <param name="psm">Tesseract Page Segmentation Mode. Varsayılan: 6 (tek uniform blok).</param>
    /// <returns>OCR sonucu olarak okunan metin. Başarısızsa boş string.</returns>
    public string Read(Mat image, string psm = "6")
    {
        return _engine.Read(image, psm);
    }

    public void Dispose()
    {
        _engine.Dispose();
    }
}