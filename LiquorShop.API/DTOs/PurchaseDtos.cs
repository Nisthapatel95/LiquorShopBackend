namespace LiquorShop.API.DTOs;

public class PurchaseOrderItemDto
{
    public int     ProductId   { get; set; }
    public string  ProductName { get; set; } = string.Empty;
    public int     Quantity    { get; set; }
    public decimal UnitPrice   { get; set; }
    public decimal LineTotal   { get; set; }
}

public class CreatePurchaseOrderDto
{
    public int                       SupplierId    { get; set; }
    public string                    SupplierName  { get; set; } = string.Empty;  // used for auto-create
    public string                    InvoiceNumber { get; set; } = string.Empty;
    public DateTime                  InvoiceDate   { get; set; }
    public int?                      OcrScanId     { get; set; }
    public List<PurchaseOrderItemDto> Items        { get; set; } = new();
}

public class PurchaseOrderDto
{
    public int                        Id            { get; set; }
    public string                     InvoiceNumber { get; set; } = string.Empty;
    public DateTime                   InvoiceDate   { get; set; }
    public decimal                    TotalAmount   { get; set; }
    public string                     Status        { get; set; } = string.Empty;
    public string                     SupplierName  { get; set; } = string.Empty;
    public DateTime                   CreatedAt     { get; set; }
    public List<PurchaseOrderItemDto> Items         { get; set; } = new();
}
