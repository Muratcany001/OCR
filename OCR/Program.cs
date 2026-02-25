using System.Diagnostics;
using OCR.Features.OCVFeatures;

namespace OCR;

class Program
{
    static void Main(string[] args)
    {
        string filePath = "/Users/murat/RiderProjects/OCR/OCR/ExamplePhotos/DA3155805_202412181150384510.bmp";
        var result = Ocv.OcvComprasion(filePath);
        
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
        }
        else
        {
            var output = $"{result.Box?.BatchNo}{result.Box?.MfgDate}{result.Box?.ExpDate}";
            Console.WriteLine(output);
        }
    }
}