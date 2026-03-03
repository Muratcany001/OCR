namespace OCR.Entities;

/// <summary>
/// Kutu etiketi (DataMatrix olmayan) görselden OCR ile okunan ürün bilgilerini tutan entity.
/// Tarih alanlarındaki '/' karakterleri setter'da otomatik olarak temizlenir.
/// </summary>
public class BoxEntity
{
    private string _batchNo = string.Empty;
    public string BatchNo { get => _batchNo; set => _batchNo = value?.Replace("O","0").ToUpper() ?? string.Empty; }
    private string _mfgDate = string.Empty;
    public string MfgDate { get => _mfgDate ; set => _mfgDate = value?.ToUpper() ?? string.Empty; }
    private string _expDate = string.Empty;
    public string ExpDate { get => _expDate; set  => _expDate = value?.ToUpper() ?? string.Empty; }
    private string _price = string.Empty;
    public string Price { get => _price; set => _price = value?.ToUpper() ?? string.Empty; }
    
}