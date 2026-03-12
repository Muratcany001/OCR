using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using OCR.Features.DatamatrixFeatures;
using OCR.Features.OCRFeatures.DatasetFactory;
using OCR.Features.OCVFeatures;
using OCR.Helpers.OutputHelpers;
using OCR.Packages;
using OpenCvSharp;
using OCR.Features.OCVFeatures;
using OCR.Helpers.DatamatrixHelpers;
using OCR.Runners;

namespace OCR;

class Program
{
    static void Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddTransient<DatamatrixFinder>();
        services.AddTransient<DatamatrixReader>();
        services.AddTransient<CreateOcrDataset.DatasetBuilder>(); 
        services.AddTransient<Ocv>();
        services.AddTransient<OcrRunner>();
        
        var serviceProvider = services.BuildServiceProvider();

        var runner = serviceProvider.GetRequiredService<OcrRunner>();
        
        runner.Run();
    }
}