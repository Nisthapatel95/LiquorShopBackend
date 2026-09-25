using LiquorShop.API.Data;
using LiquorShop.API.Data.Entities;
using LiquorShop.API.DTOs;
using LiquorShop.API.Services;
using LiquorShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LiquorShop.API.Controllers;

/// <summary>Purchase order management — OCR-verified supplier bills.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IStockService _stock;
    private readonly AuditService _audit;

    public PurchaseOrdersController(AppDbContext db, IStockService stock, AuditService audit)
    {
        _db    = db;
        _stock = stock;
        _audit = audit;
    }

    private int CurrentUserId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    /// <summary>List all purchase orders.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _db.PurchaseOrders
            .Include(p => p.Supplier)
            .Include(p => p.Items).ThenInclude(i => i.Product)
            .Select(p => new PurchaseOrderDto
            {
                Id            = p.Id,
                InvoiceNumber = p.InvoiceNumber,
                InvoiceDate   = p.InvoiceDate,
                TotalAmount   = p.TotalAmount,
                Status        = p.Status.ToString(),
                SupplierName  = p.Supplier.Name,
                CreatedAt     = p.CreatedAt,
                Items         = p.Items.Select(i => new PurchaseOrderItemDto
                {
                    ProductId   = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity    = i.Quantity,
                    UnitPrice   = i.UnitPrice,
                    LineTotal   = i.Quantity * i.UnitPrice
                }).ToList()
            })
            .ToListAsync();

        return Ok(orders);
    }

    /// <summary>Get a single purchase order by id.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _db.PurchaseOrders
            .Include(p => p.Supplier)
            .Include(p => p.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(p => p.Id == id);

        return order is null ? NotFound() : Ok(order);
    }

    /// <summary>
    /// Confirm a purchase order: auto-creates supplier/products if missing,
    /// adds stock for every line item, and records transactions.
    /// </summary>
    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmPurchase([FromBody] CreatePurchaseOrderDto dto)
    {
        // ── 1. Auto-create supplier if SupplierId == 0 ────────────────────────
        if (dto.SupplierId == 0)
        {
            var supplierName = string.IsNullOrWhiteSpace(dto.SupplierName) ? "Unknown Supplier" : dto.SupplierName.Trim();
            var existing = await _db.Suppliers.FirstOrDefaultAsync(s => s.Name == supplierName);
            if (existing is not null)
            {
                dto.SupplierId = existing.Id;
            }
            else
            {
                var newSupplier = new Supplier { Name = supplierName, IsActive = true };
                _db.Suppliers.Add(newSupplier);
                await _db.SaveChangesAsync();
                dto.SupplierId = newSupplier.Id;
            }
        }

        var order = new PurchaseOrder
        {
            SupplierId    = dto.SupplierId,
            InvoiceNumber = dto.InvoiceNumber,
            InvoiceDate   = dto.InvoiceDate,
            OcrScanId     = dto.OcrScanId,
            Status        = PurchaseOrderStatus.Confirmed
        };

        // ── 2. For each item: auto-create product if ProductId == 0 ───────────
        foreach (var item in dto.Items)
        {
            int productId = item.ProductId;

            if (productId == 0)
            {
                // Try to find existing product by name (case-insensitive)
                var name = item.ProductName.Trim();
                var existing = await _db.Products
                    .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower() && p.IsActive);

                if (existing is not null)
                {
                    productId = existing.Id;
                    // Update purchase price if it has changed
                    if (existing.PurchasePrice != item.UnitPrice)
                    {
                        existing.PurchasePrice = item.UnitPrice;
                        // Default selling price: purchase price + 30% margin if not set
                        if (existing.SellingPrice == 0)
                            existing.SellingPrice = Math.Round(item.UnitPrice * 1.30m, 2);
                        await _db.SaveChangesAsync();
                    }
                }
                else
                {
                    // Auto-create new product from bill line item
                    var newProduct = new Product
                    {
                        Name          = name,
                        SKU           = GenerateSku(name),
                        Brand         = string.Empty,
                        Unit          = "Bottle",
                        PurchasePrice = item.UnitPrice,
                        SellingPrice  = Math.Round(item.UnitPrice * 1.30m, 2), // 30% margin default
                        ReorderLevel  = 10,
                        CurrentStock  = 0,       // stock will be added below
                        CategoryId    = await GetOrCreateDefaultCategoryAsync(),
                        SupplierId    = dto.SupplierId,
                        IsActive      = true
                    };
                    _db.Products.Add(newProduct);
                    await _db.SaveChangesAsync();
                    productId = newProduct.Id;

                    await _audit.LogAsync(CurrentUserId, "Create", "Product", productId,
                        "", $"AutoCreated from invoice={dto.InvoiceNumber},Name={name}");
                }
            }

            order.Items.Add(new PurchaseOrderItem
            {
                ProductId = productId,
                Quantity  = item.Quantity,
                UnitPrice = item.UnitPrice
            });
        }

        order.TotalAmount = order.Items.Sum(i => i.Quantity * i.UnitPrice);

        _db.PurchaseOrders.Add(order);
        await _db.SaveChangesAsync();

        // ── 3. Add stock for each item ─────────────────────────────────────────
        foreach (var item in order.Items)
            await _stock.AddStockAsync(item.ProductId, item.Quantity, order.Id, $"Purchase Order #{order.InvoiceNumber}");

        await _audit.LogAsync(CurrentUserId, "Create", "PurchaseOrder", order.Id, "",
            $"Invoice={order.InvoiceNumber},Total={order.TotalAmount}");

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, new { order.Id, order.TotalAmount });
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static string GenerateSku(string name)
    {
        // e.g. "Bacardi White Rum" → "BCKWHI-001"
        var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var prefix = string.Concat(words.Take(2).Select(w => w.Length >= 3 ? w[..3].ToUpper() : w.ToUpper()));
        return $"{prefix}-{Random.Shared.Next(100, 999)}";
    }

    private async Task<int> GetOrCreateDefaultCategoryAsync()
    {
        var cat = await _db.Categories.FirstOrDefaultAsync(c => c.Name == "General");
        if (cat is not null) return cat.Id;
        var newCat = new Category { Name = "General", Description = "Auto-created default category" };
        _db.Categories.Add(newCat);
        await _db.SaveChangesAsync();
        return newCat.Id;
    }
}
