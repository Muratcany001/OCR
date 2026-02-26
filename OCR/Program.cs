using System;
using System.Diagnostics;
using OCR.Features.OCVFeatures;

namespace OCR;

class Program
{
    static void Main(string[] args)
    {
        string filePath = "/Users/murat/RiderProjects/OCR/OCR/ExamplePhotos/test.png";
        Stopwatch stopwatch = Stopwatch.StartNew();
        stopwatch.Start();
        var result = Ocv.OcvComprasion(filePath);
        stopwatch.Stop();
        
        Console.WriteLine("Stopwatch time"+stopwatch.ElapsedMilliseconds);
        if (!result.IsReadable)
        {
            Console.WriteLine("Kod okunamadı");
            return;
        }
        
        if (result.HasDataMatrix)
        {
            var output = $"{result.DataMatrix?.Gtin}{result.DataMatrix?.Sn}" +
                         $"{result.DataMatrix?.Lot}{result.DataMatrix?.Man}{result.DataMatrix?.ExpDate}";
            Console.WriteLine(output);
            Console.WriteLine("Hasdatamatrix");
        }
        else
        {
            var output = $"{result.Box?.BatchNo}{result.Box?.MfgDate}{result.Box?.ExpDate}";
            Console.WriteLine(output);
            Console.WriteLine("Hasdatamatrix");
        }
    }
}