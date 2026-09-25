namespace LiquorShop.API.Data.Entities;

public enum TransactionType
{
    Purchase,    // Stock In  (+)
    Sale,        // Stock Out (-)
    Adjustment   // Manual correction (+/-)
}

public class StockTransaction
{
    public int             Id              { get; set; }
    public TransactionType TransactionType { get; set; }
    public int             Quantity        { get; set; }   // positive = in, negative = out
    public int             ReferenceId     { get; set; }   // PurchaseOrderId or SalesOrderId
    public string          ReferenceType   { get; set; } = string.Empty;
    public string          Note            { get; set; } = string.Empty;
    public DateTime        CreatedAt       { get; set; } = DateTime.UtcNow;

    public int     ProductId { get; set; }
    public Product Product   { get; set; } = null!;
}
