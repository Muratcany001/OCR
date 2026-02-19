using System.Runtime.InteropServices;
using OpenCvSharp;

namespace OCR;

public sealed class CharacterRecognition
{
    private readonly string _lang;
    private readonly string _tesseractPath;

    public CharacterRecognition(string lang = "eng")
    {
        _lang = lang;
        _tesseractPath = GetTesseractPath();
    }
    //Tesseract ocr kutuphanesi
    private string GetTesseractPath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return @"C:\Program Files\Tesseract-OCR\tesseract.exe"; 
            
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return "/opt/homebrew/bin/tesseract"; 
            
        return "/usr/bin/tesseract"; 
    }

    // bosluklarin arasini da almak icin psm degerini 6 yaptik
    public string Read(Mat image, string psm = "6") 
    {
        
        string tmpIn = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.bmp");
        string tmpOutBase = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        string resultFile = tmpOutBase + ".txt";

        try
        {
            Cv2.ImWrite(tmpIn, image);

            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = _tesseractPath, 
                Arguments = $"{tmpIn} {tmpOutBase} -l {_lang} --psm {psm}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = System.Diagnostics.Process.Start(psi)!;
            process.WaitForExit();

            // Sadece başarısız olursa error logunu oku (Performans için)
            if (process.ExitCode != 0)
            {
                string error = process.StandardError.ReadToEnd();
                Console.WriteLine($"Tesseract Hatası: {error}");
            }

            return File.Exists(resultFile) ? File.ReadAllText(resultFile).Trim() : string.Empty;
        }
        finally
        {
            if (File.Exists(tmpIn)) File.Delete(tmpIn);
            if (File.Exists(resultFile)) File.Delete(resultFile);
        }
    }
}