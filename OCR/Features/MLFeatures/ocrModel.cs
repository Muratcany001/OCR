using Microsoft.ML.OnnxRuntime.Tensors;
using System.IO;
using Microsoft.ML.OnnxRuntime;
namespace OCR.Features.MLFeatures;

public class ocrModel
{
    public static void OcrModel()
    {
        // // add model path inside of inferencesession
        // var session = new InferenceSession("");
        // float[] imageData = new float[1 * 1 * 28 * 28];
        // var inputTensor = new DenseTensor<float>(new float[] {}, new int[] { 1, 1, 28, 28 });
        // var inputs = new List<NamedOnnxValue>{ NamedOnnxValue.CreateFromTensor("input", inputTensor) };
        // using var results = session.Run(inputs);
        // var output = results.First().AsEnumerable<float>().ToArray();
    }
    
}