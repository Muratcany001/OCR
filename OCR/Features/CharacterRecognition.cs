using System.Runtime.InteropServices;
using OCR.Packages;
using OpenCvSharp;

namespace OCR;

public sealed class CharacterRecognition
{
    private readonly string _lang;
    private readonly string _tesseractPath;

    public CharacterRecognition(string lang = "eng")
    {
        _lang = lang;
        _tesseractPath = TesseractHelper.GetTesseractPath();
    }

    // bosluklarin arasini da almak icin psm degerini 6 yapildi
    public string Read(Mat image, string psm = "6") 
    {
        string tmpIn = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
        
        try
        {
            //gorseli diske yazdir
            Cv2.ImWrite(tmpIn, image,
                new ImageEncodingParam(ImwriteFlags.PngCompression, 1));
            
            
            //process parametreleri
            // lstm only ve whitelist tanimlandi
            // bloklar teker teker islendigi icin psm 6
            // 
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = _tesseractPath, 
                Arguments = $"{tmpIn} stdout -l {_lang} --oem 1 --psm {psm} -c tessedit_char_whitelist=0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ:/",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8,
                StandardErrorEncoding = System.Text.Encoding.UTF8,
            };
            //kurulan processi baslatma
            using var process = System.Diagnostics.Process.Start(psi)!;
            
            string result = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            
            process.WaitForExit();

            //process kontrolu
            if (process.ExitCode != 0)
            {
                Console.WriteLine($"Tesseract Hatası: {error}");
            }
            //cikti okuma
            return result.Trim();
        }
        finally
        {
            // olusturulan dosyalarin silinmesi
            if (File.Exists(tmpIn))
                File.Delete(tmpIn);
        }
    }
}