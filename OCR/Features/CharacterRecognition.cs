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
    //Cross platform tesseract kurulumu
    private string GetTesseractPath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return @"C:\Program Files\Tesseract-OCR\tesseract.exe"; 
            
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return "/opt/homebrew/bin/tesseract"; 
            
        return "/usr/bin/tesseract"; 
    }

    // bosluklarin arasini da almak icin psm degerini 6 yapildi
    public string Read(Mat image, string psm = "6") 
    {
        
        string tmpIn = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.bmp");
        string tmpOutBase = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        string resultFile = tmpOutBase + ".txt";
        
        try
        {
            //gorseli diske yazdirma
            Cv2.ImWrite(tmpIn, image,
                new ImageEncodingParam(ImwriteFlags.PngCompression, 1));
            //process parametreleri
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = _tesseractPath, 
                Arguments = $"{tmpIn} {tmpOutBase} -l {_lang} --oem 1 --psm {psm} -c tessedit_char_whitelist=0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ:/",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            //kurulan processi baslatma
            using var process = System.Diagnostics.Process.Start(psi)!;
            process.WaitForExit();

            //process kontrolu
            if (process.ExitCode != 0)
            {
                string error = process.StandardError.ReadToEnd();
                Console.WriteLine($"Tesseract Hatası: {error}");
            }
            //cikti okuma
            return File.Exists(resultFile) ? File.ReadAllText(resultFile).Trim() : string.Empty;
        }
        finally
        {
            if (File.Exists(tmpIn))
                File.Delete(tmpIn);
        }
    }
}