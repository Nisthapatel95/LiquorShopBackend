using LiquorShop.API.Data;
using LiquorShop.API.DTOs;
using LiquorShop.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LiquorShop.API.Controllers;

/// <summary>Product catalogue management.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly AuditService _audit;
    public ProductsController(AppDbContext db, AuditService audit) { _db = db; _audit = audit; }

    private int CurrentUserId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    /// <summary>Get all active products.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _db.Products
            .Where(p => p.IsActive)
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Select(p => new ProductDto
            {
                Id            = p.Id,
                Name          = p.Name,
                SKU           = p.SKU,
                Barcode       = p.Barcode,
                Brand         = p.Brand,
                Unit          = p.Unit,
                PurchasePrice = p.PurchasePrice,
                SellingPrice  = p.SellingPrice,
                CurrentStock  = p.CurrentStock,
                ReorderLevel  = p.ReorderLevel,
                IsActive      = p.IsActive,
                CategoryName  = p.Category.Name,
                SupplierName  = p.Supplier != null ? p.Supplier.Name : string.Empty
            })
            .ToListAsync();

        return Ok(products);
    }

    /// <summary>Get product by id.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var p = await _db.Products.Include(p => p.Category).Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.Id == id);
        return p is null ? NotFound() : Ok(p);
    }

    /// <summary>Create a new product (Admin).</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var product = new Data.Entities.Product
        {
            Name          = dto.Name,
            SKU           = dto.SKU,
            Barcode       = dto.Barcode,
            Brand         = dto.Brand,
            Unit          = dto.Unit,
            PurchasePrice = dto.PurchasePrice,
            SellingPrice  = dto.SellingPrice,
            ReorderLevel  = dto.ReorderLevel,
            CategoryId    = dto.CategoryId,
            SupplierId    = dto.SupplierId
        };
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        await _audit.LogAsync(CurrentUserId, "Create", "Product", product.Id, "", $"Name={product.Name},SKU={product.SKU}");
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>Update a product (Admin).</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return NotFound();

        product.Name          = dto.Name;
        product.SKU           = dto.SKU;
        product.Barcode       = dto.Barcode;
        product.Brand         = dto.Brand;
        product.Unit          = dto.Unit;
        product.PurchasePrice = dto.PurchasePrice;
        product.SellingPrice  = dto.SellingPrice;
        product.ReorderLevel  = dto.ReorderLevel;
        product.CategoryId    = dto.CategoryId;
        product.SupplierId    = dto.SupplierId;
        product.IsActive      = dto.IsActive;

        await _db.SaveChangesAsync();
        await _audit.LogAsync(CurrentUserId, "Update", "Product", id, "", $"Name={dto.Name}");
        return NoContent();
    }

    /// <summary>Soft-delete a product (Admin).</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return NotFound();
        product.IsActive = false;
        await _db.SaveChangesAsync();
        await _audit.LogAsync(CurrentUserId, "Delete", "Product", id, $"Name={product.Name}", "");
        return NoContent();
    }

    /// <summary>Get products at or below reorder level.</summary>
    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock()
    {
        var items = await _db.Products
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

        return Ok(items);
    }
}
