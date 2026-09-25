namespace LiquorShop.API.Data.Entities;

public class Product
{
    public int     Id            { get; set; }
    public string  Name          { get; set; } = string.Empty;
    public string  SKU           { get; set; } = string.Empty;
    public string  Barcode       { get; set; } = string.Empty;
    public string  Brand         { get; set; } = string.Empty;
    public string  Unit          { get; set; } = "Bottle";       // Bottle, Case, Can
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice  { get; set; }
    public int     CurrentStock  { get; set; }
    public int     ReorderLevel  { get; set; } = 10;
    public bool    IsActive      { get; set; } = true;
    public DateTime CreatedAt    { get; set; } = DateTime.UtcNow;

    public int      CategoryId { get; set; }
    public Category Category   { get; set; } = null!;

    public int?     SupplierId { get; set; }
    public Supplier? Supplier  { get; set; }

    public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
    public ICollection<SalesOrderItem>    SalesOrderItems    { get; set; } = new List<SalesOrderItem>();
    public ICollection<StockTransaction>  StockTransactions  { get; set; } = new List<StockTransaction>();
}
