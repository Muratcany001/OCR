namespace OCR.Entities;

/// <summary>
/// Kutu etiketi (DataMatrix olmayan) görselden OCR ile okunan ürün bilgilerini tutan entity.
/// Tarih alanlarındaki '/' karakterleri setter'da otomatik olarak temizlenir.
/// </summary>
public class BoxEntity
{
    public string BatchNo { get; set; }
    private string _mfgDate;
    public string MfgDate { get => _mfgDate ; set => _mfgDate = value?.Replace("/",""); }
    private string _expDate;
    public string ExpDate { get => _expDate; set  => _expDate = value?.Replace("/",""); }
    public string? Price { get; set; }
    
}