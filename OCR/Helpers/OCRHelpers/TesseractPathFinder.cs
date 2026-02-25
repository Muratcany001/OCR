using System.Runtime.InteropServices;

namespace OCR.Packages;

/// <summary>
/// Tesseract OCR binary ve tessdata dizininin konumunu tespit eden yardımcı sınıf.
/// Windows, macOS ve Linux platformlarını destekler.
/// </summary>
public class TesseractPathFinder
{
    /// <summary>
    /// Tesseract binary'sinin dosya yolunu bulur.
    /// Önce PATH'te arar, bulamazsa platform bazlı bilinen konumları dener.
    /// </summary>
    /// <returns>Tesseract binary'sinin mutlak dosya yolu.</returns>
    /// <exception cref="Exception">Tesseract bulunamazsa kurulum talimatı ile fırlatılır.</exception>
    public static string GetTesseractPath()
    {
        var fromPath = FindInPath(
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "tesseract.exe"
                : "tesseract");

        if (fromPath != null) return fromPath;

        string[] candidates = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? [@"C:\Program Files\Tesseract-OCR\tesseract.exe",
                @"C:\Program Files (x86)\Tesseract-OCR\tesseract.exe"]
            : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                ? ["/opt/homebrew/bin/tesseract", "/usr/local/bin/tesseract"]
                : ["/usr/bin/tesseract", "/usr/local/bin/tesseract"];

        return candidates.FirstOrDefault(File.Exists)
               ?? throw new Exception("Tesseract bulunamadı.\n" +
                                      "macOS: brew install tesseract\n" +
                                      "Linux: sudo apt install tesseract-ocr\n" +
                                      "Windows: https://github.com/UB-Mannheim/tesseract/wiki");
    }
    
    /// <summary>
    /// Tesseract dil verisi (tessdata) dizininin yolunu bulur.
    /// Önce TESSDATA_PREFIX environment variable'ını kontrol eder.
    /// </summary>
    /// <returns>tessdata dizininin mutlak yolu.</returns>
    /// <exception cref="Exception">tessdata bulunamazsa fırlatılır.</exception>
    public static string GetTessDataPath()
    {
        var env = Environment.GetEnvironmentVariable("TESSDATA_PREFIX");
        if (!string.IsNullOrEmpty(env) && Directory.Exists(env)) return env;

        string[] candidates = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? [@"C:\Program Files\Tesseract-OCR\tessdata"]
            : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                ? ["/opt/homebrew/share/tessdata", "/usr/local/share/tessdata"]
                : ["/usr/share/tesseract-ocr/5/tessdata", "/usr/share/tessdata"];

        return candidates.FirstOrDefault(Directory.Exists)
               ?? throw new Exception("tessdata bulunamadi. TESSDATA_PREFIX environment variable'ini ayarlayin.");
    }

    /// <summary>
    /// PATH environment variable'ındaki dizinlerde belirtilen binary'yi arar.
    /// </summary>
    /// <param name="binary">Aranacak binary dosya adı (ör: "tesseract").</param>
    /// <returns>Binary'nin mutlak yolu veya bulunamazsa null.</returns>
    private static string? FindInPath(string binary)
    {
        var path = Environment.GetEnvironmentVariable("PATH") ?? "";
        return path.Split(Path.PathSeparator)
            .Select(dir => Path.Combine(dir, binary))
            .FirstOrDefault(File.Exists);
    }
}