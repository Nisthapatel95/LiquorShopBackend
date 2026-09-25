using LiquorShop.API.Data;
using LiquorShop.API.Data.Entities;
using LiquorShop.API.DTOs;
using LiquorShop.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiquorShop.API.Services;

public class StockService : IStockService
{
    private readonly AppDbContext _db;
    public StockService(AppDbContext db) => _db = db;

    /// <summary>Stock In: purchase received.</summary>
    public async Task AddStockAsync(int productId, int quantity, int referenceId, string note)
    {
        var product = await _db.Products.FindAsync(productId)
            ?? throw new KeyNotFoundException($"Product {productId} not found.");

        product.CurrentStock += quantity;

        _db.StockTransactions.Add(new StockTransaction
        {
            ProductId       = productId,
            TransactionType = TransactionType.Purchase,
            Quantity        = quantity,
            ReferenceId     = referenceId,
            ReferenceType   = "PurchaseOrder",
            Note            = note
        });

        await _db.SaveChangesAsync();
    }

    /// <summary>Manual stock adjustment (+/-).</summary>
    public async Task AdjustStockAsync(int productId, int quantity, string note)
    {
        var product = await _db.Products.FindAsync(productId)
            ?? throw new KeyNotFoundException($"Product {productId} not found.");

        if (product.CurrentStock + quantity < 0)
            throw new InvalidOperationException($"Adjustment would result in negative stock. Current: {product.CurrentStock}");

        product.CurrentStock += quantity;

        _db.StockTransactions.Add(new StockTransaction
        {
            ProductId       = productId,
            TransactionType = TransactionType.Adjustment,
            Quantity        = quantity,
            ReferenceId     = 0,
            ReferenceType   = "ManualAdjustment",
            Note            = note
        });

        await _db.SaveChangesAsync();
    }

    /// <summary>Stock Out: customer sale.</summary>
    public async Task DeductStockAsync(int productId, int quantity, int referenceId, string note)
    {
        var product = await _db.Products.FindAsync(productId)
            ?? throw new KeyNotFoundException($"Product {productId} not found.");

        if (product.CurrentStock < quantity)
            throw new InvalidOperationException($"Insufficient stock for product {productId}. Available: {product.CurrentStock}");

        product.CurrentStock -= quantity;

        _db.StockTransactions.Add(new StockTransaction
        {
            ProductId       = productId,
            TransactionType = TransactionType.Sale,
            Quantity        = -quantity,
            ReferenceId     = referenceId,
            ReferenceType   = "SalesOrder",
            Note            = note
        });

        await _db.SaveChangesAsync();
    }

    public async Task<List<StockReportDto>> GetCurrentStockAsync()
    {
        return await _db.Products
            .Where(p => p.IsActive)
            .Select(p => new StockReportDto
            {
                ProductId    = p.Id,
                ProductName  = p.Name,
                SKU          = p.SKU,
                CurrentStock = p.CurrentStock,
                ReorderLevel = p.ReorderLevel,
                TotalIn      = _db.StockTransactions
                                  .Where(t => t.ProductId == p.Id && t.Quantity > 0)
                                  .Sum(t => t.Quantity),
                TotalOut     = _db.StockTransactions
                                  .Where(t => t.ProductId == p.Id && t.Quantity < 0)
                                  .Sum(t => -t.Quantity)
            })
            .ToListAsync();
    }
}
