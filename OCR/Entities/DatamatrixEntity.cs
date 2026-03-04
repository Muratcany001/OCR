namespace OCR.Entities;

/// <summary>
/// DataMatrix barkodundan okunan ürün bilgilerini tutan entity.
/// GS1 standartlarına uygun alanlar içerir.
/// </summary>
public class DatamatrixEntity
{
    private string _lot = string.Empty;
    public string Lot { get => _lot; set => _lot = value?.Replace("O","0").ToUpper() ?? string.Empty; }
    
    public string Gtin { get; set; } = string.Empty;
    public string Sn { get; set; }
    
    private string _expdate = string.Empty;
    public string ExpDate { get => _expdate ; set => _expdate = value?.Replace("/","") ?? string.Empty; }
}