namespace LiquorShop.API.Data.Entities;

public class PurchaseOrderItem
{
    public int     Id              { get; set; }
    public int     Quantity        { get; set; }
    public decimal UnitPrice       { get; set; }
    public decimal LineTotal       => Quantity * UnitPrice;

    public int           PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder   { get; set; } = null!;

    public int     ProductId { get; set; }
    public Product Product   { get; set; } = null!;
}
