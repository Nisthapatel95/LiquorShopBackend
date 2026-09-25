namespace LiquorShop.API.DTOs;

// ── User DTOs ─────────────────────────────────────────────────────────────────

public class UserDto
{
    public int      Id        { get; set; }
    public string   FullName  { get; set; } = string.Empty;
    public string   Email     { get; set; } = string.Empty;
    public string   Role      { get; set; } = string.Empty;
    public int      RoleId    { get; set; }
    public bool     IsActive  { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateUserDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email    { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int    RoleId   { get; set; } = 2; // default: Cashier
}

public class ResetPasswordDto
{
    public string NewPassword { get; set; } = string.Empty;
}

// ── Category DTOs ─────────────────────────────────────────────────────────────

public class CategoryDto
{
    public int    Id           { get; set; }
    public string Name         { get; set; } = string.Empty;
    public string Description  { get; set; } = string.Empty;
    public int    ProductCount { get; set; }
}

public class CreateCategoryDto
{
    public string Name        { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

// ── Stock Adjustment DTO ──────────────────────────────────────────────────────

public class StockAdjustDto
{
    public int    ProductId { get; set; }
    public int    Quantity  { get; set; }  // positive = add, negative = remove
    public string Note      { get; set; } = string.Empty;
}

