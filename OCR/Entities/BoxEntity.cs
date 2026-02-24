namespace OCR.Entities;

public class BoxEntity
{
    public string? BatchNo { get; set; }
    private string _mfgDate;
    public string? MfgDate { get => _mfgDate ; set => _mfgDate = value?.Replace("/",""); }
    private string _expDate;
    public string? ExpDate { get => _expDate; set  => _expDate = value?.Replace("/",""); }
    public string? Price { get; set; }
    
}