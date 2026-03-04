using OpenCvSharp;
using OCR.Packages;

namespace OCR.Features.OCRFeatures;

public sealed class MacOcrEngine : IOcrEngine
{
    private readonly string _lang;
    private readonly string _tesseractPath;

    public MacOcrEngine(string lang)
    {
        _lang = lang;
        _tesseractPath = TesseractPathFinder.GetTesseractPath();
    }

    public string Read(Mat image, string psm = "6")
    {
        if (image == null || image.Empty())
            return string.Empty;
    
        string tmpIn = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.bmp");
        
        try
        {
            // BMP = sıfır compression overhead, PNG'den çok daha hızlı yazılır
            Cv2.ImWrite(tmpIn, image);
            
            //process parametreleri
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = _tesseractPath, 
                Arguments = $"{tmpIn} stdout -l {_lang} --oem 2 --psm {psm} -c tessedit_char_whitelist=0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ.:/",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8,
                StandardErrorEncoding = System.Text.Encoding.UTF8,
            };
            
            using var process = System.Diagnostics.Process.Start(psi)!;
            
            var resultTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();
            process.WaitForExit();
            
            string result = resultTask.Result;
            string error = errorTask.Result;

            if (process.ExitCode != 0)
            {
                Console.WriteLine($"Tesseract Hatası: {error}");
            }
            return result.Trim();
        }
        finally
        {
            if (File.Exists(tmpIn))
                File.Delete(tmpIn);
        }
    }

    public void Dispose()
    {
    }
}
