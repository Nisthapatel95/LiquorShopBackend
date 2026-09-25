using LiquorShop.API.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LiquorShop.API.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext db)
    {
        // 1. Ensure Roles exist
        if (!await db.Roles.AnyAsync())
        {
            db.Roles.AddRange(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Cashier" }
            );
            await db.SaveChangesAsync();
        }

        // 2. Ensure Admin User exists
        if (!await db.Users.AnyAsync(u => u.Username == "admin" || u.Email == "admin@liquorshop.com"))
        {
            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@liquorshop.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                FullName = "Administrator",
                Phone = "9999999999",
                RoleId = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            db.Users.Add(adminUser);
            await db.SaveChangesAsync();
        }

        // 3. Ensure Cashier User exists
        if (!await db.Users.AnyAsync(u => u.Username == "cashier" || u.Email == "cashier@liquorshop.com"))
        {
            var cashierUser = new User
            {
                Username = "cashier",
                Email = "cashier@liquorshop.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Cashier@123"),
                FullName = "Default Cashier",
                Phone = "8888888888",
                RoleId = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            db.Users.Add(cashierUser);
            await db.SaveChangesAsync();
        }

        // 4. Ensure Default Categories exist
        if (!await db.Categories.AnyAsync())
        {
            var categories = new[]
            {
                new Category { Name = "Whiskey", Description = "Premium Aged Spirits" },
                new Category { Name = "Beer", Description = "Domestic and Imported Beers" },
                new Category { Name = "Rum", Description = "White and Dark Rums" },
                new Category { Name = "Vodka", Description = "Distilled Spirits" },
                new Category { Name = "Wine", Description = "Red and White Wines" },
                new Category { Name = "General", Description = "Default Category" }
            };
            db.Categories.AddRange(categories);
            await db.SaveChangesAsync();
        }

        // 5. Ensure Default Suppliers exist
        if (!await db.Suppliers.AnyAsync())
        {
            var suppliers = new[]
            {
                new Supplier { Name = "Martignetti Companies", ContactPerson = "Sales Dept", Email = "orders@martignetti.com", Phone = "781-761-3950", IsActive = true },
                new Supplier { Name = "National Beverage Distributors", ContactPerson = "Logistics", Email = "contact@natbev.com", Phone = "800-555-0199", IsActive = true }
            };
            db.Suppliers.AddRange(suppliers);
            await db.SaveChangesAsync();
        }
    }
}
