using System.Diagnostics;
using OCR.Features.DatamatrixFeatures;
using OCR.Helpers.DatamatrixHelpers;
using OpenCvSharp;

namespace OCR.Features.OCRFeatures.DatasetFactory;

public class CreateDatamatrixDataset
{
    private readonly DatamatrixReader _reader;
    private readonly DatamatrixFinder _finder;

    public CreateDatamatrixDataset(DatamatrixReader datamatrixReader, DatamatrixFinder datamatrixFinder)
    {
        _reader = datamatrixReader;
        _finder = datamatrixFinder;
    }
    public void DatamatrixDataset(string filePath)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        var rawSrc = Cv2.ImRead(filePath);
        Mat src = new Mat(rawSrc);
        var dmRect = _finder.FindDataMatrix(src);
        Mat dm = src[dmRect];
        if (dm.Width > 0 & dm.Height > 0)
        {
            Cv2.ImWrite($"/Users/murat/RiderProjects/OCR/OCR/Outputs/DatamatrixOutputs/{filePath}.jpg", dm);
        
            var readerResult = _reader.ReadDataMatrix(rawSrc, dmRect);
            File.WriteAllText(filePath, readerResult.ToString());
        }
        else
        {
            Console.WriteLine("Datamatrix okunamadi");
        }
        stopwatch.Stop();
        Console.WriteLine($"Dataset'e yazilma suresi:  {stopwatch.ElapsedMilliseconds} ms");
    }
}