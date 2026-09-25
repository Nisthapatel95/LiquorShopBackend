namespace LiquorShop.API.DTOs;

public class StockReportDto
{
    public int     ProductId    { get; set; }
    public string  ProductName  { get; set; } = string.Empty;
    public string  SKU          { get; set; } = string.Empty;
    public int     CurrentStock { get; set; }
    public int     ReorderLevel { get; set; }
    public int     TotalIn      { get; set; }
    public int     TotalOut     { get; set; }
}

public class SalesSummaryDto
{
    public DateTime Date         { get; set; }
    public int      TotalOrders  { get; set; }
    public decimal  TotalRevenue { get; set; }
    public decimal  TotalTax     { get; set; }
    public decimal  NetRevenue   { get; set; }
}

public class LowStockDto
{
    public int    ProductId    { get; set; }
    public string ProductName  { get; set; } = string.Empty;
    public string SKU          { get; set; } = string.Empty;
    public int    CurrentStock { get; set; }
    public int    ReorderLevel { get; set; }
}

public class PurchaseSummaryDto
{
    public DateTime Date           { get; set; }
    public int      TotalOrders    { get; set; }
    public decimal  TotalPurchased { get; set; }
}
