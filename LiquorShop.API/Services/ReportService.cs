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
        var toDate = to.Date.AddDays(1).AddTicks(-1);
        return await _db.SalesOrders
            .Where(s => s.CreatedAt >= from.Date && s.CreatedAt <= toDate)
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
        var toDate = to.Date.AddDays(1).AddTicks(-1);
        return await _db.PurchaseOrders
            .Where(p => p.InvoiceDate >= from.Date && p.InvoiceDate <= toDate)
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

    public async Task<List<ProductPurchaseReportDto>> GetProductPurchasesAsync(DateTime from, DateTime to)
    {
        var toDate = to.Date.AddDays(1).AddTicks(-1);
        var items = await _db.PurchaseOrderItems
            .Include(i => i.PurchaseOrder)
            .Include(i => i.Product)
                .ThenInclude(p => p.Category)
            .Include(i => i.Product)
                .ThenInclude(p => p.Supplier)
            .Where(i => i.PurchaseOrder.InvoiceDate >= from.Date && i.PurchaseOrder.InvoiceDate <= toDate)
            .ToListAsync();

        return items
            .GroupBy(i => new { Date = i.PurchaseOrder.InvoiceDate.Date, i.ProductId })
            .Select(g =>
            {
                var first = g.First();
                var totalQty = g.Sum(i => i.Quantity);
                var totalAmt = g.Sum(i => i.LineTotal);
                return new ProductPurchaseReportDto
                {
                    Date          = g.Key.Date,
                    ProductId     = g.Key.ProductId,
                    ProductName   = first.Product?.Name ?? "Unknown Product",
                    SKU           = first.Product?.SKU ?? string.Empty,
                    CategoryName  = first.Product?.Category?.Name ?? "N/A",
                    SupplierName  = first.PurchaseOrder?.Supplier?.Name ?? first.Product?.Supplier?.Name ?? "N/A",
                    TotalQuantity = totalQty,
                    AvgUnitPrice  = totalQty > 0 ? Math.Round(totalAmt / totalQty, 2) : 0,
                    TotalAmount   = totalAmt
                };
            })
            .OrderByDescending(r => r.Date)
            .ThenBy(r => r.ProductName)
            .ToList();
    }

    public async Task<List<DailyProductSalesDto>> GetDailyProductSalesAsync(DateTime from, DateTime to)
    {
        var toDate = to.Date.AddDays(1).AddTicks(-1);
        var items = await _db.SalesOrderItems
            .Include(i => i.SalesOrder)
            .Include(i => i.Product)
                .ThenInclude(p => p.Category)
            .Where(i => i.SalesOrder.CreatedAt >= from.Date && i.SalesOrder.CreatedAt <= toDate)
            .ToListAsync();

        return items
            .GroupBy(i => new { Date = i.SalesOrder.CreatedAt.Date, i.ProductId })
            .Select(g =>
            {
                var first = g.First();
                var totalQty = g.Sum(i => i.Quantity);
                var totalRev = g.Sum(i => i.LineTotal);
                return new DailyProductSalesDto
                {
                    Date            = g.Key.Date,
                    ProductId       = g.Key.ProductId,
                    ProductName     = first.Product?.Name ?? "Unknown Product",
                    SKU             = first.Product?.SKU ?? string.Empty,
                    CategoryName    = first.Product?.Category?.Name ?? "N/A",
                    TotalQuantity   = totalQty,
                    AvgSellingPrice = totalQty > 0 ? Math.Round(totalRev / totalQty, 2) : 0,
                    TotalRevenue    = totalRev
                };
            })
            .OrderByDescending(r => r.Date)
            .ThenByDescending(r => r.TotalRevenue)
            .ToList();
    }

    public async Task<List<BestSellingProductDto>> GetBestSellingProductsAsync(DateTime from, DateTime to, int top = 20)
    {
        var toDate = to.Date.AddDays(1).AddTicks(-1);
        var items = await _db.SalesOrderItems
            .Include(i => i.SalesOrder)
            .Include(i => i.Product)
                .ThenInclude(p => p.Category)
            .Where(i => i.SalesOrder.CreatedAt >= from.Date && i.SalesOrder.CreatedAt <= toDate)
            .ToListAsync();

        return items
            .GroupBy(i => i.ProductId)
            .Select(g =>
            {
                var first = g.First();
                return new BestSellingProductDto
                {
                    ProductId         = g.Key,
                    ProductName       = first.Product?.Name ?? "Unknown Product",
                    SKU               = first.Product?.SKU ?? string.Empty,
                    CategoryName      = first.Product?.Category?.Name ?? "N/A",
                    TotalQuantitySold = g.Sum(i => i.Quantity),
                    TotalRevenue      = g.Sum(i => i.LineTotal),
                    TotalOrders       = g.Select(i => i.SalesOrderId).Distinct().Count()
                };
            })
            .OrderByDescending(r => r.TotalQuantitySold)
            .ThenByDescending(r => r.TotalRevenue)
            .Take(top)
            .ToList();
    }
}
