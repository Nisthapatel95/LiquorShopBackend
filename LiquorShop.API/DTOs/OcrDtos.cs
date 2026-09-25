namespace LiquorShop.API.DTOs;

public class OcrLineItemDto
{
    public string  ProductName { get; set; } = string.Empty;
    public int     Quantity    { get; set; }
    public decimal UnitPrice   { get; set; }
}

public class OcrResultDto
{
    public int               ScanId        { get; set; }
    public string            SupplierName  { get; set; } = string.Empty;
    public string            InvoiceNumber { get; set; } = string.Empty;
    public DateTime          InvoiceDate   { get; set; }
    public List<OcrLineItemDto> LineItems  { get; set; } = new();
    public string            RawText       { get; set; } = string.Empty;
}

/// <summary>Result of a fully-automatic scan + confirm operation.</summary>
public class AutoConfirmResultDto
{
    public bool             Success          { get; set; }
    public string           ErrorMessage     { get; set; } = string.Empty;
    public int              PurchaseOrderId  { get; set; }
    public string           InvoiceNumber    { get; set; } = string.Empty;
    public string           SupplierName     { get; set; } = string.Empty;
    public decimal          TotalAmount      { get; set; }
    public int              ItemCount        { get; set; }
    public List<string>     NewProductsAdded { get; set; } = new();
    public OcrResultDto?    OcrResult        { get; set; }
}
