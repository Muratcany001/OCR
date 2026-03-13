using System.Diagnostics;
using Features.OCRFeatures.ImageProcessing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
        services.AddTransient<Gs1Parser>();
        services.AddTransient<GetDataMatrix>();
        services.AddTransient<TesseractPathFinder>();
        services.AddTransient<WarmupHelper>();
        services.AddTransient<OutputParser>();
        services.AddTransient<RegexHelper>();
        services.AddTransient<CharacterRecognition>();
        services.AddTransient<ImageProcessing>();
        services.AddTransient<CreateOcrDataset.DatasetBuilder>(); 
        services.AddTransient<Ocv>();
        services.AddTransient<OcrRunner>();
        
        var serviceProvider = services.BuildServiceProvider();

        var runner = serviceProvider.GetRequiredService<OcrRunner>();
        
        runner.Run();
    }
}