namespace LiquorShop.API.Data.Entities;

public class User
{
    public int    Id           { get; set; }
    public string FullName     { get; set; } = string.Empty;
    public string Email        { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool   IsActive     { get; set; } = true;
    public DateTime CreatedAt  { get; set; } = DateTime.UtcNow;

    public int  RoleId { get; set; }
    public Role Role   { get; set; } = null!;

    public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
    public ICollection<AuditLog>   AuditLogs   { get; set; } = new List<AuditLog>();
}
