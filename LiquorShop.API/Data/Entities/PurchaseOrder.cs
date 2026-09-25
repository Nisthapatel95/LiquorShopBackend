namespace LiquorShop.API.Data.Entities;

public enum PurchaseOrderStatus
{
    Draft,
    PendingVerification,
    Confirmed,
    Cancelled
}

public class PurchaseOrder
{
    public int                 Id            { get; set; }
    public string              InvoiceNumber { get; set; } = string.Empty;
    public DateTime            InvoiceDate   { get; set; }
    public decimal             TotalAmount   { get; set; }
    public PurchaseOrderStatus Status        { get; set; } = PurchaseOrderStatus.Draft;
    public string              Notes         { get; set; } = string.Empty;
    public DateTime            CreatedAt     { get; set; } = DateTime.UtcNow;

    public int      SupplierId { get; set; }
    public Supplier Supplier   { get; set; } = null!;

    public int?     OcrScanId  { get; set; }
    public OcrScan? OcrScan    { get; set; }

    public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
}
