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
        var result = Ocv.OcvComprasion(filePath);
        stopwatch.Stop();

        Console.WriteLine($"Total OCV time: {stopwatch.ElapsedMilliseconds}ms");

        if (!result.IsReadable)
        {
            Console.WriteLine("Kod okunamadı");
            return;
        }

        if (result.HasDataMatrix)
        {
            var output = $"{result.DataMatrix?.Gtin}{result.DataMatrix?.Sn}" +
                         $"{result.DataMatrix?.Lot}{result.DataMatrix?.Man}{result.DataMatrix?.ExpDate}";

            var parsedOutput = OutputParser.DatamatrixParse(output);
            Console.WriteLine($"GTIN: {parsedOutput?.Gtin}");
        }
        else
        {
            var output = $"{result.Box?.BatchNo}{result.Box?.MfgDate}{result.Box?.ExpDate}";
            var parsedOutput = OutputParser.OCRParse(output);
            Console.WriteLine($"Batch No: {parsedOutput?.BatchNo}");
            Console.WriteLine($"Raw output: {output}");
        }

        Console.WriteLine($"Raw OCR text: {result.RawOcrText}");
    }
}