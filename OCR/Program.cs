using System.Diagnostics;
using OCR.Features.OCVFeatures;
using OCR.Helpers.OutputHelpers;
using OCR.Packages;
using OpenCvSharp;
using OCR.Features.OCVFeatures;
namespace OCR;

class Program
{
    static void Main(string[] args)
    {
        Stopwatch globalWatch = Stopwatch.StartNew();
    
        WarmupHelper.Warmup();
    
        // Klasördeki tüm .bmp dosyalarını al
        string folderPath = "/Users/murat/RiderProjects/OCR/OCR/ExamplePhotos";
        string[] filePaths = Directory.GetFiles(folderPath, "*.bmp");

        Console.WriteLine($"{filePaths.Length} adet dosya bulundu. İşlem başlıyor...\n");
        
        foreach (var filePath in filePaths)
        {
            // Her fotoğraf için ayrı bir süre tutmak istersen (isteğe bağlı)
            Stopwatch fileWatch = Stopwatch.StartNew();
        
            Console.WriteLine($"--- İşleniyor: {Path.GetFileName(filePath)} ---");

            var result = Ocv.AnalyzeImage(filePath);
            // Skoru hesapla
            var score = Validator.ResultValidator(result);
            // Çıktıları Bas
            if (result.HasDataMatrix)
            {
                // DataMatrix tarihini senin OCR formatına çeviren o mantığı 
                // ResultValidator içinde veya burada yapmayı unutma!
                Console.WriteLine($"Ocr benzerlik skoru: %{score}");
                Console.WriteLine($"Ocroutput: {result.DatamatrixOcrOutput} | dmOutput: {result.DatamatrixOutput}");
            }
            else
            {
                Console.WriteLine("Exp date" + result.Box?.ExpDate);
                Console.WriteLine($"Mfg date: {result.Box?.MfgDate}");
                Console.WriteLine("Batch no "+result.Box?.BatchNo);
            }

            fileWatch.Stop();
            Console.WriteLine($"Dosya İşleme Süresi: {fileWatch.ElapsedMilliseconds} ms");
            Console.WriteLine("---------------------------------------------------\n");
        }

        // Tesseract engine'i en son kapatıyoruz (her döngüde kapatıp açmak yavaşlatır)
        Ocv.Ocr.Dispose();

        globalWatch.Stop();
        Console.WriteLine($"\n>>> TOPLAM ÇALIŞMA SÜRESİ: {globalWatch.Elapsed}");
        Console.WriteLine($">>> Ortalama Hız: {globalWatch.ElapsedMilliseconds / filePaths.Length} ms/fotoğraf");
    }
}