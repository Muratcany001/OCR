using System.Diagnostics;
using OCR.Features.OCVFeatures;
using OCR.Helpers.OutputHelpers;

namespace OCR;

class Program
{
    static void Main(string[] args)
    {
        string filePath = "/Users/murat/RiderProjects/OCR/OCR/ExamplePhotos/DA3155805_202412181150384510.bmp";
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
        
        if (result.DataMatrix != null)
        {
            Console.WriteLine(OutputParser.ToStringOutput(result.DataMatrix));
            Console.WriteLine(result.DataMatrix.Lot);
        }
        else
        {
            var output = $"{result.Box?.BatchNo}{result.Box?.MfgDate}{result.Box?.ExpDate}";
            Console.WriteLine(output);
        }
    }
}