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

public class ProductPurchaseReportDto
{
    public DateTime Date          { get; set; }
    public int      ProductId     { get; set; }
    public string   ProductName   { get; set; } = string.Empty;
    public string   SKU           { get; set; } = string.Empty;
    public string   CategoryName  { get; set; } = string.Empty;
    public string   SupplierName  { get; set; } = string.Empty;
    public int      TotalQuantity { get; set; }
    public decimal  AvgUnitPrice  { get; set; }
    public decimal  TotalAmount   { get; set; }
}

public class DailyProductSalesDto
{
    public DateTime Date          { get; set; }
    public int      ProductId     { get; set; }
    public string   ProductName   { get; set; } = string.Empty;
    public string   SKU           { get; set; } = string.Empty;
    public string   CategoryName  { get; set; } = string.Empty;
    public int      TotalQuantity { get; set; }
    public decimal  AvgSellingPrice { get; set; }
    public decimal  TotalRevenue  { get; set; }
}

public class BestSellingProductDto
{
    public int     ProductId     { get; set; }
    public string  ProductName   { get; set; } = string.Empty;
    public string  SKU           { get; set; } = string.Empty;
    public string  CategoryName  { get; set; } = string.Empty;
    public int     TotalQuantitySold { get; set; }
    public decimal TotalRevenue  { get; set; }
    public int     TotalOrders   { get; set; }
}


