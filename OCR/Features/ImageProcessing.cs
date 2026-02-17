using System.Diagnostics;
using OpenCvSharp;

namespace OCR;

public class ImageProcessing
{
    static void ProcessFile(string filePath)
    {
     // input section   
     Stopwatch sw = Stopwatch.StartNew();
     byte[] rawImage = File.ReadAllBytes(filePath);
     
     // roi section before grayscale 
     Rect roi = new Rect(100, 200, 300, 50);
     using (var croppedImage = rawImage.Submat(roi))
     {
         
     }
     
     
    }
}