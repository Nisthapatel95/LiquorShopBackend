namespace LiquorShop.API.Data.Entities;

public class SalesOrder
{
    public int      Id          { get; set; }
    public string   OrderNumber { get; set; } = string.Empty;
    public decimal  TotalAmount { get; set; }
    public decimal  Discount    { get; set; }
    public decimal  TaxAmount   { get; set; }
    public decimal  NetAmount   { get; set; }
    public string   PaymentMode   { get; set; } = "Cash";  // Cash, Card, UPI
    public string   CustomerName  { get; set; } = string.Empty;
    public string   CustomerPhone { get; set; } = string.Empty;
    public DateTime CreatedAt     { get; set; } = DateTime.UtcNow;

    public int  CashierId { get; set; }
    public User Cashier   { get; set; } = null!;

    public ICollection<SalesOrderItem> Items { get; set; } = new List<SalesOrderItem>();
}
