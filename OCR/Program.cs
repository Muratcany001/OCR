using System.Diagnostics;
using OCR.Features.OCVFeatures;
using OCR.Helpers.OutputHelpers;

namespace OCR;

class Program
{
    static void Main(string[] args)
    {
        string filePath = "/Users/murat/RiderProjects/OCR/OCR/ExamplePhotos/dm/dm3.bmp";
        
        Stopwatch stopwatch = Stopwatch.StartNew();
        stopwatch.Start();
        var result = Ocv.AnalyzeImage(filePath);
        stopwatch.Stop();
        Console.WriteLine("Stopwatch time"+stopwatch.ElapsedMilliseconds);
        
        if (!result.IsReadable)
        {
            Console.WriteLine("Kod okunamadı veya eksik bilgi (IsReadable = false)\n");
        }
        
        Console.WriteLine("--- Ham OCR Metni ---");
        Console.WriteLine(result.RawOcrText);

        if (result.HasDataMatrix)
        {
            Console.WriteLine("\n--- DataMatrix Çıktısı ---");
            Console.WriteLine($"SN:   {result.DataMatrix?.Sn}");
            Console.WriteLine("Gtin "+ result.DataMatrix?.Gtin);
            Console.WriteLine("Lot "+ result.DataMatrix?.Lot);
            
            Console.WriteLine("\n--- OCR Çıktısı (Aynı Görselden Regex ile) ---");
            Console.WriteLine($"SN:   {result.OcrData?.Sn}");
            Console.WriteLine($"Gtin:   {result.OcrData?.Gtin}");
            Console.WriteLine($"Lot:   {result.OcrData?.Lot}");
        }
        else
        {
            Console.WriteLine("\n--- Kutu OCR Çıktısı ---");
            Console.WriteLine($"Batch No: {result.Box?.BatchNo}");
            Console.WriteLine($"Mfg Date: {result.Box?.MfgDate}");
            Console.WriteLine($"Exp Date: {result.Box?.ExpDate}");
            Console.WriteLine($"Price:    {result.Box?.Price}");
        }
    }
}