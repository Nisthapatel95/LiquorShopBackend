using LiquorShop.API.Data;
using LiquorShop.API.DTOs;
using LiquorShop.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiquorShop.API.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _db;
    public ReportService(AppDbContext db) => _db = db;

    public async Task<List<SalesSummaryDto>> GetSalesSummaryAsync(DateTime from, DateTime to)
    {
        return await _db.SalesOrders
            .Where(s => s.CreatedAt >= from && s.CreatedAt <= to)
            .GroupBy(s => s.CreatedAt.Date)
            .Select(g => new SalesSummaryDto
            {
                Date         = g.Key,
                TotalOrders  = g.Count(),
                TotalRevenue = g.Sum(s => s.TotalAmount),
                TotalTax     = g.Sum(s => s.TaxAmount),
                NetRevenue   = g.Sum(s => s.NetAmount)
            })
            .OrderBy(r => r.Date)
            .ToListAsync();
    }

    public async Task<List<PurchaseSummaryDto>> GetPurchaseSummaryAsync(DateTime from, DateTime to)
    {
        return await _db.PurchaseOrders
            .Where(p => p.InvoiceDate >= from && p.InvoiceDate <= to)
            .GroupBy(p => p.InvoiceDate.Date)
            .Select(g => new PurchaseSummaryDto
            {
                Date           = g.Key,
                TotalOrders    = g.Count(),
                TotalPurchased = g.Sum(p => p.TotalAmount)
            })
            .OrderBy(r => r.Date)
            .ToListAsync();
    }

    public async Task<List<LowStockDto>> GetLowStockAsync()
    {
        return await _db.Products
            .Where(p => p.IsActive && p.CurrentStock <= p.ReorderLevel)
            .Select(p => new LowStockDto
            {
                ProductId    = p.Id,
                ProductName  = p.Name,
                SKU          = p.SKU,
                CurrentStock = p.CurrentStock,
                ReorderLevel = p.ReorderLevel
            })
            .OrderBy(p => p.CurrentStock)
            .ToListAsync();
    }
}
