namespace OCR.Entities;

/// <summary>
/// DataMatrix barkodundan okunan ürün bilgilerini tutan entity.
/// GS1 standartlarına uygun alanlar içerir.
/// </summary>
public class DatamatrixEntity
{
    private string _lot;
    public string Lot { get => _lot; set => _lot = value?.Replace("O","0").ToUpper(); }
    private string _man;
    public string Man { get => _man; set => _man = value?.Replace("/", ""); }
    public string Gtin { get; set; }
    public string Sn { get; set; }
    
    private string _expdate;
    public string ExpDate { get => _expdate ; set => _expdate = value?.Replace("/",""); }
}