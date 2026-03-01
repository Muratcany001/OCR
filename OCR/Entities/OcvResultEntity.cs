namespace OCR.Entities;

public class OcvResultEntity
{
    /// <summary>
    /// OcvComprasion metodunun sonuç sınıfı.
    /// </summary>
    public class OcvResult
    {
        /// <summary>Zorunlu alanlar okunabildi mi?</summary>
        public bool IsReadable { get; set; }
        public bool HasDataMatrix { get; set; }
        public DatamatrixEntity? DataMatrix { get; set; }
        public BoxEntity? Box { get; set; }
        public string RawOcrText { get; set; } = "";
        public DatamatrixEntity OcrData { get; set; }
    }
}