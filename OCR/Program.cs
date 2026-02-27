using OCR.Features.OCVFeatures;
using OCR.Helpers.OutputHelpers;
using OpenCvSharp;
using System.Diagnostics;

namespace OCR;

class Program
{

    static void Main(string[] args)
    {
        string filePath = @"C:\Users\murat\source\repos\OCR\OCR\test.bmp";

        Stopwatch stopwatch = Stopwatch.StartNew();
        var result = Ocv.AnalyzeImage(filePath);
        stopwatch.Stop();

        Console.WriteLine($"Total OCV time: {stopwatch.ElapsedMilliseconds}ms");

        if (!result.IsReadable)
        {
            Console.WriteLine("Kod okunamadı");
            return;
        }

        if (result.HasDataMatrix)
        {
            Console.WriteLine($"GTIN: {result.DataMatrix?.Gtin}");
            Console.WriteLine($"SN: {result.DataMatrix?.Sn}");
            Console.WriteLine($"LOT: {result.DataMatrix?.Lot}");
            Console.WriteLine($"MAN: {result.DataMatrix?.Man}");
            Console.WriteLine($"EXP: {result.DataMatrix?.ExpDate}");
            Console.WriteLine(result.DataMatrix?.Gtin);
        }
        else
        {
            Console.WriteLine($"Batch No: {result.Box?.BatchNo}");
            Console.WriteLine($"Mfg Date: {result.Box?.MfgDate}");
            Console.WriteLine($"Exp Date: {result.Box?.ExpDate}");
        }
    }
}