namespace LiquorShop.API.Data.Entities;

public enum OcrScanStatus
{
    Pending,
    Processed,
    Failed
}

public class OcrScan
{
    public int          Id            { get; set; }
    public string       FileName      { get; set; } = string.Empty;
    public string       FilePath      { get; set; } = string.Empty;
    public string       ExtractedJson { get; set; } = string.Empty;
    public OcrScanStatus Status       { get; set; } = OcrScanStatus.Pending;
    public DateTime     CreatedAt     { get; set; } = DateTime.UtcNow;

    public PurchaseOrder? PurchaseOrder { get; set; }
}
