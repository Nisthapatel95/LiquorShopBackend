namespace LiquorShop.API.DTOs;

public class CartItemDto
{
    public int     ProductId { get; set; }
    public int     Quantity  { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CreateSalesOrderDto
{
    public int              CashierId     { get; set; }
    public string           CashierName   { get; set; } = string.Empty;
    public string           CustomerName  { get; set; } = string.Empty;
    public string           CustomerPhone { get; set; } = string.Empty;
    public decimal          Discount      { get; set; }
    public decimal          TaxAmount     { get; set; }
    public string           PaymentMode   { get; set; } = "Cash";
    public List<CartItemDto> Items        { get; set; } = new();
}

public class SalesOrderItemDto
{
    public int     ProductId   { get; set; }
    public string  ProductName { get; set; } = string.Empty;
    public int     Quantity    { get; set; }
    public decimal UnitPrice   { get; set; }
    public decimal LineTotal   { get; set; }
}

public class SalesOrderDto
{
    public int                   Id            { get; set; }
    public string                OrderNumber   { get; set; } = string.Empty;
    public string                CustomerName  { get; set; } = string.Empty;
    public string                CustomerPhone { get; set; } = string.Empty;
    public decimal               TotalAmount   { get; set; }
    public decimal               Discount      { get; set; }
    public decimal               TaxAmount     { get; set; }
    public decimal               NetAmount     { get; set; }
    public string                PaymentMode   { get; set; } = string.Empty;
    public string                CashierName   { get; set; } = string.Empty;
    public DateTime              CreatedAt     { get; set; }
    public List<SalesOrderItemDto> Items       { get; set; } = new();
}
