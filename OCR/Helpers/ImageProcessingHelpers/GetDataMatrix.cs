using OpenCvSharp;

namespace OCR.Packages;

public class GetDataMatrix
{
    public static Mat DatamatrixImage(Mat src)
    {
        var rects = DatamatrixFinder.FindDataMatrix(src);
        int margin = 10;
        // datamatrix cevresini kes
        Rect expanded = new Rect(
            rects.X -margin,
            rects.Y -margin,
            rects.Width + margin*2,
            rects.Height + margin*2);
        // kare nesnesi ile width height x y ayarla
        Rect safeRect = expanded & new Rect(0, 0, src.Width, src.Height);
        // mevcut resimden datamatrixi kirp
        Mat dmBinary = new Mat(src, safeRect);
        return dmBinary;
    }
}