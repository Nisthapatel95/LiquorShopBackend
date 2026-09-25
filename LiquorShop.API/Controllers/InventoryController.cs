using LiquorShop.API.DTOs;
using LiquorShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LiquorShop.API.Controllers;

/// <summary>Inventory stock view and movement history.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IStockService _stock;
    private readonly LiquorShop.API.Data.AppDbContext _db;

    public InventoryController(IStockService stock, LiquorShop.API.Data.AppDbContext db)
    {
        _stock = stock;
        _db    = db;
    }

    /// <summary>Current stock snapshot for all products.</summary>
    [HttpGet("current-stock")]
    public async Task<IActionResult> GetCurrentStock() =>
        Ok(await _stock.GetCurrentStockAsync());

    /// <summary>Manual stock adjustment (Admin only).</summary>
    [HttpPost("adjust")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Adjust([FromBody] StockAdjustDto dto)
    {
        try
        {
            await _stock.AdjustStockAsync(dto.ProductId, dto.Quantity, dto.Note);
            return Ok(new { message = "Stock adjusted successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>All stock transactions (movements log).</summary>
    [HttpGet("stock-movements")]
    public async Task<IActionResult> GetStockMovements()
    {
        var movements = await _db.StockTransactions
            .Include(t => t.Product)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new
            {
                t.Id,
                t.TransactionType,
                t.Quantity,
                t.ReferenceId,
                t.ReferenceType,
                t.Note,
                t.CreatedAt,
                ProductName = t.Product.Name,
                ProductSKU  = t.Product.SKU
            })
            .ToListAsync();

        return Ok(movements);
    }
}
