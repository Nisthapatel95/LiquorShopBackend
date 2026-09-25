namespace LiquorShop.API.Data.Entities;

public class AuditLog
{
    public int      Id         { get; set; }
    public string   Action     { get; set; } = string.Empty;  // Create, Update, Delete
    public string   Entity     { get; set; } = string.Empty;  // e.g. "Product"
    public int      EntityId   { get; set; }
    public string   OldValues  { get; set; } = string.Empty;  // JSON
    public string   NewValues  { get; set; } = string.Empty;  // JSON
    public DateTime CreatedAt  { get; set; } = DateTime.UtcNow;

    public int  UserId { get; set; }
    public User User   { get; set; } = null!;
}
