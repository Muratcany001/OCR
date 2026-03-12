using System.Diagnostics;
using OCR.Features.DatamatrixFeatures;
using OCR.Features.OCRFeatures.DatasetFactory;
using OCR.Features.OCVFeatures;
using OCR.Helpers.DatamatrixHelpers;
using OCR.Packages;

namespace OCR.Runners;

public class OcrRunner
{
    private readonly  DatamatrixReader _reader;
    private readonly DatamatrixFinder _finder;
    private readonly Ocv _ocv;
    private readonly CreateOcrDataset.DatasetBuilder _createOcrDataset;
    public OcrRunner(DatamatrixReader reader, DatamatrixFinder finder , CreateOcrDataset.DatasetBuilder createOcrDataset, Ocv ocv)
    {
        _createOcrDataset = createOcrDataset;
        _ocv = ocv;
        _reader = reader;
        _finder = finder;
    }
    public void Run()
    {
        Stopwatch globalWatch = Stopwatch.StartNew();
    
        WarmupHelper.Warmup();
    
        // Klasördeki tüm .bmp dosyalarını al
        string folderPath = "/Users/murat/RiderProjects/OCR/OCR/ExamplePhotos";
        string[] filePaths = Directory.GetFiles(folderPath, "*.bmp");

        Console.WriteLine($"{filePaths.Length} adet dosya bulundu. İşlem başlıyor...\n");
        
        string outputFolder = @"/Users/murat/RiderProjects/OCR/OCR/Features/OCRFeatures/DatasetFactory/output";
        string inputFolder = @"/Users/murat/RiderProjects/OCR/OCR/ExamplePhotos";
        
        _createOcrDataset.BuildDataset(inputFolder, outputFolder);
        
        foreach (var filePath in filePaths)
        {
            Stopwatch fileWatch = Stopwatch.StartNew();
        
            Console.WriteLine($"--- İşleniyor: {Path.GetFileName(filePath)} ---");

            var result = _ocv.AnalyzeImage(filePath);
            var score = Validator.ResultValidator(result);
            if (result.HasDataMatrix)
            {
                Console.WriteLine($"Ocr benzerlik skoru: %{score}");
                Console.WriteLine($"Ocroutput: {result.DatamatrixOcrOutput} | dmOutput: {result.DatamatrixOutput}");
            }
            else if (!string.IsNullOrWhiteSpace(result.OcrData?.Gtin) &&
                     !string.IsNullOrWhiteSpace(result.OcrData?.Sn))
            {
                Console.WriteLine($"Ocr benzerlik skoru: %{score}");
                Console.WriteLine($"Ocroutput: {result.DatamatrixOcrOutput}");
            }
            else if (result.Box != null && (!string.IsNullOrWhiteSpace(result.Box.BatchNo) ||
                                             !string.IsNullOrWhiteSpace(result.Box.ExpDate)))
            {
                // Box verisi 
                Console.WriteLine("Exp date: "  + result.Box.ExpDate);
                Console.WriteLine("Mfg date: "  + result.Box.MfgDate);
                Console.WriteLine("Batch no: "  + result.Box.BatchNo);
            }
            else
            {
                // Null
                Console.WriteLine("[HATA] Hiçbir veri çıkarılamadı.");
                Console.WriteLine($"  RawOcrText: {(string.IsNullOrWhiteSpace(result.RawOcrText) ? "<boş>" : result.RawOcrText.Replace("\n", " | "))}");
            }

            fileWatch.Stop();
            Console.WriteLine($"Dosya İşleme Süresi: {fileWatch.ElapsedMilliseconds} ms");
            Console.WriteLine("---------------------------------------------------\n");
        }
        
        Ocv.Ocr.Dispose();

        globalWatch.Stop();
        Console.WriteLine($"\n>>> TOPLAM ÇALIŞMA SÜRESİ: {globalWatch.Elapsed}");
        Console.WriteLine($">>> Ortalama Hız: {globalWatch.ElapsedMilliseconds / filePaths.Length} ms/fotoğraf");
    }
}