using NumSharp;
using OCR.Features.DatamatrixFeatures;
using OCR.Features.OCVFeatures;
using OCR.Helpers.DatamatrixHelpers;
using OpenCvSharp;

namespace OCR.Features.OCRFeatures.DatasetFactory;

public class CreateOcrDataset
{
    // <summary>
    /// OCR + DataMatrix pipeline'ını kullanarak
    /// görüntüleri .npy ve etiketleri .txt olarak kaydeder.
    /// </summary>
    public class DatasetBuilder
    {
        private readonly DatamatrixReader _reader;
        private readonly DatamatrixFinder _finder;
        private readonly Ocv _ocv;

        public DatasetBuilder(DatamatrixReader reader, DatamatrixFinder finder, Ocv ocv)
        {
            _reader = reader;
            _finder = finder;
            _ocv = ocv;
        }
        /// <summary>
        /// Verilen klasördeki tüm resimleri işler ve dataset klasörüne kaydeder.
        /// </summary>
        /// <param name="inputFolder">Resimlerin bulunduğu klasör</param>
        /// <param name="outputFolder">Çıktı dataset klasörü</param>
        public void BuildDataset(string inputFolder, string outputFolder)
        {
            var inputDir = new DirectoryInfo(inputFolder);
            if (!inputDir.Exists) throw new DirectoryNotFoundException(inputFolder);

            Directory.CreateDirectory(outputFolder);
            var imagesFolder = Path.Combine(outputFolder, "images");
            var labelsFolder = Path.Combine(outputFolder, "labels");
            Directory.CreateDirectory(imagesFolder);
            Directory.CreateDirectory(labelsFolder);

            var files = inputDir.GetFiles("*.bmp")
                                .Concat(inputDir.GetFiles("*.bmp"))
                                .ToArray();

            foreach (var file in files)
            {
                Console.WriteLine($"Processing {file.Name}...");
                var result = _ocv.AnalyzeImage(file.FullName);

                // Görüntüyü grayscale ve normalize et
                using var imgMat = Cv2.ImRead(file.FullName, ImreadModes.Grayscale);
                var resized = imgMat.Resize(new OpenCvSharp.Size(128, 32));
                int rows = resized.Rows;
                int cols = resized.Cols;
                
                float[,] data = new float[rows, cols];
                
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        data[r, c] = resized.At<byte>(r, c) / 255.0f; // normalize
                    }
                }
                var npArray = np.array(data);

                // .npy olarak kaydet
                string npyPath = Path.Combine(imagesFolder, file.Name.Replace(file.Extension, ".npy"));
                np.save(npyPath, npArray);

                // Etiket olarak DataMatrix veya OCR verisini al
                string label = result.HasDataMatrix && result.DataMatrix != null
                    ? $"{result.DataMatrix.Lot}{result.DataMatrix.ExpDate}{result.DataMatrix.Sn}"
                    : result.RawOcrText ?? string.Empty;

                // Eğer label boşsa, işaretle "CHECK"
                if (string.IsNullOrWhiteSpace(label))
                    label = "CHECK";

                // .txt olarak kaydet
                string txtPath = Path.Combine(labelsFolder, file.Name.Replace(file.Extension, ".txt"));
                File.WriteAllText(txtPath, label);

                Console.WriteLine($"Saved .npy -> {npyPath}, .txt -> {txtPath}");
            }

            Console.WriteLine("Dataset generation completed.");
        }
    }
}