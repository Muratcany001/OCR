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
        /// <summary>Box görseli OCR'ında BatchNo başarıyla parse edilebildi mi?</summary>
        public bool HasBatchNo { get; set; }
        public DatamatrixEntity? DataMatrix { get; set; }
        public BoxEntity? Box { get; set; }
        public string RawOcrText { get; set; } = "";
        public DatamatrixEntity OcrData { get; set; } = new();
        
        public string DatamatrixOcrOutput { get; set; } = "";
        public string DatamatrixOutput { get; set; } = "";
        public string BoxOcrOutput { get; set; } = "";
    }
}