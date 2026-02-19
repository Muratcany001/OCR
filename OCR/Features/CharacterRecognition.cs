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
        //islenecek dosya 
        string tmpIn = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.bmp");
        //tesseract output prefix
        string tmpOutBase = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        //olusacak txt file
        string resultFile = tmpOutBase + ".txt";
        
        try
        {
            //gorseli diske yazdir
            Cv2.ImWrite(tmpIn, image,
                new ImageEncodingParam(ImwriteFlags.PngCompression, 1));
            
            
            //process parametreleri
            // lstm only ve whitelist tanimlandi
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
            // input silinmesi
            if (File.Exists(tmpIn))
                File.Delete(tmpIn);
        }
    }
}