using System.Runtime.InteropServices;
using OpenCvSharp;

namespace OCR.Features.OCRFeatures;

public sealed class MacNativeOcrEngine : IOcrEngine
{
    // Mac'te Homebrew ile kurulan tesseract'ın ana kütüphane adı
    private const string TesseractLib = "/opt/homebrew/Cellar/tesseract/5.5.2/lib/libtesseract.5.dylib";

    [DllImport(TesseractLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr TessBaseAPICreate();

    [DllImport(TesseractLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern int TessBaseAPIInit3(IntPtr handle, string datapath, string language);

    [DllImport(TesseractLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern void TessBaseAPISetImage(IntPtr handle, IntPtr imagedata, int width, int height, int bytes_per_pixel, int bytes_per_line);

    [DllImport(TesseractLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr TessBaseAPIGetUTF8Text(IntPtr handle);

    [DllImport(TesseractLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern void TessBaseAPIDelete(IntPtr handle);
    
    [DllImport(TesseractLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern int TessBaseAPISetVariable(IntPtr handle, string name, string value);

    [DllImport(TesseractLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern void TessBaseAPISetPageSegMode(IntPtr handle, int mode);

    private readonly IntPtr _handle;

    public MacNativeOcrEngine(string lang)
    {
        // Tesseract verilerinin (tessdata) olduğu klasör yolu
        // Genellikle "/opt/homebrew/share/tessdata" olur
        string tessDataPath = "/opt/homebrew/share/tessdata/"; 

        _handle = TessBaseAPICreate();
        if (TessBaseAPIInit3(_handle, tessDataPath, lang) != 0)
        {
            throw new Exception("Tesseract Native Engine başlatılamadı! Yolunuzu kontrol edin.");
        }
    }

    public string Read(Mat image, string psm = "6")
    {
        if (image == null || image.Empty()) return string.Empty;

        // Resmi bellekte (RAM) Tesseract'a tanıt
        TessBaseAPISetImage(_handle, image.Data, image.Width, image.Height, image.ElemSize(), (int)image.Step());
        TessBaseAPISetVariable(_handle, "tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-:/");
        TessBaseAPISetPageSegMode(_handle,6);
        
        // Metni oku
        IntPtr textPtr = TessBaseAPIGetUTF8Text(_handle);
        string result = Marshal.PtrToStringAnsi(textPtr) ?? string.Empty;

        return result.Trim();
    }

    public void Dispose()
    {
        if (_handle != IntPtr.Zero)
            TessBaseAPIDelete(_handle);
    }
}