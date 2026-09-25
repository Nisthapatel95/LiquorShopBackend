using LiquorShop.API.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LiquorShop.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Role>             Roles             => Set<Role>();
    public DbSet<User>             Users             => Set<User>();
    public DbSet<Category>         Categories        => Set<Category>();
    public DbSet<Supplier>         Suppliers         => Set<Supplier>();
    public DbSet<Product>          Products          => Set<Product>();
    public DbSet<PurchaseOrder>    PurchaseOrders    => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem>PurchaseOrderItems=> Set<PurchaseOrderItem>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<SalesOrder>       SalesOrders       => Set<SalesOrder>();
    public DbSet<SalesOrderItem>   SalesOrderItems   => Set<SalesOrderItem>();
    public DbSet<OcrScan>          OcrScans          => Set<OcrScan>();
    public DbSet<AuditLog>         AuditLogs         => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Cashier" }
        );

        // Decimal precision
        modelBuilder.Entity<Product>()
            .Property(p => p.PurchasePrice).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Product>()
            .Property(p => p.SellingPrice).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<PurchaseOrder>()
            .Property(p => p.TotalAmount).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<PurchaseOrderItem>()
            .Property(p => p.UnitPrice).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<SalesOrder>()
            .Property(p => p.TotalAmount).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<SalesOrder>()
            .Property(p => p.Discount).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<SalesOrder>()
            .Property(p => p.TaxAmount).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<SalesOrder>()
            .Property(p => p.NetAmount).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<SalesOrderItem>()
            .Property(p => p.UnitPrice).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<SalesOrderItem>()
            .Property(p => p.LineTotal).HasColumnType("decimal(18,2)");

        // Avoid cascades on User → SalesOrder (multiple cascade paths)
        modelBuilder.Entity<SalesOrder>()
            .HasOne(s => s.Cashier)
            .WithMany(u => u.SalesOrders)
            .HasForeignKey(s => s.CashierId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AuditLog>()
            .HasOne(a => a.User)
            .WithMany(u => u.AuditLogs)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
