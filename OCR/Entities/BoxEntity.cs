namespace OCR.Entities;

/// <summary>
/// Kutu etiketi (DataMatrix olmayan) görselden OCR ile okunan ürün bilgilerini tutan entity.
/// Tarih alanlarındaki '/' karakterleri setter'da otomatik olarak temizlenir.
/// </summary>
public class BoxEntity
{
    private string _batchNo;
    public string BatchNo { get => _batchNo; set => _batchNo = value?.ToUpper(); }
    private string _mfgDate;
    public string MfgDate { get => _mfgDate ; set => _mfgDate = value?.Replace("/","").ToUpper(); }
    private string _expDate;
    public string ExpDate { get => _expDate; set  => _expDate = value?.Replace("/","").ToUpper(); }
    private string _price;
    public string? Price { get => _price; set => _price = value?.ToUpper(); }
    
}