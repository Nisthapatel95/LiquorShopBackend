namespace LiquorShop.API.DTOs;

public class ProductDto
{
    public int     Id            { get; set; }
    public string  Name          { get; set; } = string.Empty;
    public string  SKU           { get; set; } = string.Empty;
    public string  Barcode       { get; set; } = string.Empty;
    public string  Brand         { get; set; } = string.Empty;
    public string  Unit          { get; set; } = string.Empty;
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice  { get; set; }
    public int     CurrentStock  { get; set; }
    public int     ReorderLevel  { get; set; }
    public bool    IsActive      { get; set; }
    public string  CategoryName  { get; set; } = string.Empty;
    public string  SupplierName  { get; set; } = string.Empty;
}

public class CreateProductDto
{
    public string  Name          { get; set; } = string.Empty;
    public string  SKU           { get; set; } = string.Empty;
    public string  Barcode       { get; set; } = string.Empty;
    public string  Brand         { get; set; } = string.Empty;
    public string  Unit          { get; set; } = "Bottle";
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice  { get; set; }
    public int     ReorderLevel  { get; set; } = 10;
    public int     CategoryId    { get; set; }
    public int?    SupplierId    { get; set; }
}

public class UpdateProductDto : CreateProductDto
{
    public bool IsActive { get; set; } = true;
}
