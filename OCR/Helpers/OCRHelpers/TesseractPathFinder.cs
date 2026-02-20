using System.Runtime.InteropServices;

namespace OCR.Packages;

public class TesseractPathFinder
{
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

    private static string? FindInPath(string binary)
    {
        var path = Environment.GetEnvironmentVariable("PATH") ?? "";
        return path.Split(Path.PathSeparator)
            .Select(dir => Path.Combine(dir, binary))
            .FirstOrDefault(File.Exists);
    }
}