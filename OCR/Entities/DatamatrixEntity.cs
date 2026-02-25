namespace OCR.Entities;

/// <summary>
/// DataMatrix barkodundan okunan ürün bilgilerini tutan entity.
/// GS1 standartlarına uygun alanlar içerir.
/// </summary>
public class DatamatrixEntity
{
    private string _lot;
    public string? Lot { get => _lot; set => _lot?.ToUpper(); }
    public string? Man { get; set; }
    public string? Gtin { get; set; }
    public string? Sn { get; set; }
    
    private string _expdate;
    public string ExpDate { get => _expdate ; set => _expdate = value?.Replace("/",""); }
}