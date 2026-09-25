namespace LiquorShop.API.Data.Entities;

public class Role
{
    public int    Id   { get; set; }
    public string Name { get; set; } = string.Empty;   // Admin, Cashier

    public ICollection<User> Users { get; set; } = new List<User>();
}
