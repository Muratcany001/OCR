using System.Diagnostics;
using OpenCvSharp;

namespace OCR;

public class ImageProcessing
{
    public static void ProcessFile(string filePath)
    {
     // input section   
     Stopwatch sw = Stopwatch.StartNew();
     using(Mat src = Cv2.ImRead(filePath)){
         // image input control
         if (src.Empty()){ 
             Console.WriteLine("Image could not be loaded"); 
             return;
         }
         
         // cut roi section before grayscale 
        Rect roi = new Rect(190, 355, 590, 350);
         using (var croppedImage = src[roi])
         {
             
             // grayscale section
             using (Mat gray = new Mat())
             {
                 Cv2.CvtColor(croppedImage, gray, ColorConversionCodes.BGR2GRAY);
                 
                 // threshold section 
                 using (Mat binary = new Mat())
                 {
                      //ornek 70 threshold  Time elapsed: 00:00:00.0554831
                      //succes rate : okay
                      //Cv2.Threshold(gray,binary,70,255, ThresholdTypes.Binary);
                     
                     // otsu threshold output is 121
                     // succes rate : low Time elapsed: 00:00:00.0613221
                     // double otsuThreshold = Cv2.Threshold(gray, binary,0, 255,   ThresholdTypes.Binary | ThresholdTypes.Otsu);
                     // Console.WriteLine("Otsu threshold: " + otsuThreshold);
                     
                     // adaptive  threshold Time elapsed: 00:00:00.0562022
                     // succes rate : good
                     Cv2.AdaptiveThreshold(gray, binary, 255,
                         AdaptiveThresholdTypes.GaussianC,
                         ThresholdTypes.Binary, 21, 10  
                     );
                     
                     //stopwatch 
                     sw.Stop(); 
                     Console.WriteLine("Time elapsed: " + sw.Elapsed);
                     
                     Cv2.ImShow("Binary image",binary);
                     Cv2.WaitKey(0);
                     Cv2.DestroyAllWindows();
                 }
             }
         }
     }
    }
}