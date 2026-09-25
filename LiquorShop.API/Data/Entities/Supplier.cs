namespace LiquorShop.API.Data.Entities;

public class Supplier
{
    public int    Id          { get; set; }
    public string Name        { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Phone       { get; set; } = string.Empty;
    public string Email       { get; set; } = string.Empty;
    public string Address     { get; set; } = string.Empty;
    public bool   IsActive    { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Product>       Products       { get; set; } = new List<Product>();
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}
