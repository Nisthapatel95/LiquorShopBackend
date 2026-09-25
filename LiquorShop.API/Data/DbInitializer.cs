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

        // 2. Ensure Admin User exists with correct password
        var admin = await db.Users.FirstOrDefaultAsync(u => u.Email == "admin@liquorshop.com");
        if (admin is null)
        {
            db.Users.Add(new User
            {
                FullName     = "Administrator",
                Email        = "admin@liquorshop.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                RoleId       = 1,
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow
            });
        }
        else
        {
            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
            admin.IsActive     = true;
        }

        // 3. Ensure Cashier User exists with correct password
        var cashier = await db.Users.FirstOrDefaultAsync(u => u.Email == "cashier@liquorshop.com");
        if (cashier is null)
        {
            db.Users.Add(new User
            {
                FullName     = "Default Cashier",
                Email        = "cashier@liquorshop.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Cashier@123"),
                RoleId       = 2,
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow
            });
        }
        else
        {
            cashier.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Cashier@123");
            cashier.IsActive     = true;
        }

        await db.SaveChangesAsync();

        // 4. Ensure Default Categories exist
        if (!await db.Categories.AnyAsync())
        {
            var categories = new[]
            {
                new Category { Name = "Whiskey", Description = "Premium Aged Spirits" },
                new Category { Name = "Beer",    Description = "Domestic and Imported Beers" },
                new Category { Name = "Rum",     Description = "White and Dark Rums" },
                new Category { Name = "Vodka",   Description = "Distilled Spirits" },
                new Category { Name = "Wine",    Description = "Red and White Wines" },
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
                new Supplier { Name = "Martignetti Companies", ContactName = "Sales Dept", Email = "orders@martignetti.com", Phone = "781-761-3950", IsActive = true },
                new Supplier { Name = "National Beverage Distributors", ContactName = "Logistics", Email = "contact@natbev.com", Phone = "800-555-0199", IsActive = true }
            };
            db.Suppliers.AddRange(suppliers);
            await db.SaveChangesAsync();
        }
    }
}
