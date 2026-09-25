namespace LiquorShop.API.DTOs;

public class SupplierDto
{
    public int    Id          { get; set; }
    public string Name        { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Phone       { get; set; } = string.Empty;
    public string Email       { get; set; } = string.Empty;
    public string Address     { get; set; } = string.Empty;
    public bool   IsActive    { get; set; }
}

public class CreateSupplierDto
{
    public string Name        { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Phone       { get; set; } = string.Empty;
    public string Email       { get; set; } = string.Empty;
    public string Address     { get; set; } = string.Empty;
}
