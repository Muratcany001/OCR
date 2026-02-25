using System.Runtime.InteropServices;
using OCR.Packages;
using OpenCvSharp;

namespace OCR;

/// <summary>
/// Tesseract OCR motoru ile görselden metin okuma işlemini yöneten sınıf.
/// Görseli geçici dosyaya yazarak Tesseract CLI üzerinden işlem yapar.
/// </summary>
public sealed class CharacterRecognition
{
    private readonly string _lang;
    private readonly string _tesseractPath;

    public CharacterRecognition(string lang = "eng")
    {
        _lang = lang;
        _tesseractPath = TesseractPathFinder.GetTesseractPath();
    }

    /// <summary>
    /// Verilen Mat görselini Tesseract ile okuyarak metin döndürür.
    /// Görseli geçici PNG dosyasına yazar, Tesseract CLI'yı çalıştırır ve sonucu okur.
    /// </summary>
    /// <param name="image">OCR uygulanacak işlenmiş görsel (Mat).</param>
    /// <param name="psm">Tesseract Page Segmentation Mode. Varsayılan: 6 (tek uniform blok).</param>
    /// <returns>OCR sonucu olarak okunan metin. Başarısızsa boş string.</returns>
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
                Arguments = $"{tmpIn} stdout -l {_lang} --oem 1 --psm {psm} -c tessedit_char_whitelist=0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz./:",
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