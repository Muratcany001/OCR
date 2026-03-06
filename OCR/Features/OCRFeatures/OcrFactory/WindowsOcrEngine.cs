// =====================================================
// GEREKLI KURULUM (NuGet paketi YOK, manuel)
// =====================================================
// 1. Tesseract Windows kurulum:
//    https://github.com/UB-Mannheim/tesseract/wiki
//    Kurulum sonrası: C:\Program Files\Tesseract-OCR\tesseract50.dll
//    Bu dll'i projenin exe klasörüne kopyala
//
// 2. tessdata:
//    https://github.com/tesseract-ocr/tessdata/raw/main/eng.traineddata
//    Kopyala: C:\Program Files\tessdata\eng.traineddata
// =====================================================

using System.Runtime.InteropServices;
using OpenCvSharp;

namespace OCR.Features.OCRFeatures;

public sealed class WindowsOcrEngine : IOcrEngine
{
    private const string TesseractLib = "tesseract50.dll";

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

    private IntPtr _handle;

    public WindowsOcrEngine(string lang)
    {
        string tessDataPath = @"C:\Program Files\tessdata"; 
        if (!Directory.Exists(tessDataPath))
            throw new DirectoryNotFoundException($"tessdata folder not found: {tessDataPath}");

        _handle = TessBaseAPICreate();
        if (TessBaseAPIInit3(_handle, tessDataPath, lang) != 0)
            throw new Exception("Tesseract başlatılamadı!");

        TessBaseAPISetPageSegMode(_handle, 6);
        TessBaseAPISetVariable(_handle, "tessedit_char_whitelist",
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-:/");
        TessBaseAPISetVariable(_handle, "load_system_dawg", "0");
        TessBaseAPISetVariable(_handle, "load_freq_dawg", "0");
    }

    public string Read(Mat image, string psm = "6")
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(WindowsOcrEngine));
        if (image == null || image.Empty())
            return string.Empty;

        TessBaseAPISetImage(_handle, image.Data, image.Width, image.Height,
            image.ElemSize(), (int)image.Step());

        IntPtr textPtr = TessBaseAPIGetUTF8Text(_handle);
        return Marshal.PtrToStringAnsi(textPtr)?.Trim() ?? string.Empty;
    }

    public void Dispose()
    {
        if (_handle != IntPtr.Zero)
        {
            TessBaseAPIDelete(_handle);
            _handle = IntPtr.Zero;
        }
    }
}