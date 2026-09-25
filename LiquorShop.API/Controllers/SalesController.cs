using LiquorShop.API.Data;
using LiquorShop.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LiquorShop.API.Controllers;

/// <summary>Sales history and order detail.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly AppDbContext _db;
    public SalesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _db.SalesOrders
            .Include(s => s.Cashier)
            .Include(s => s.Items).ThenInclude(i => i.Product)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SalesOrderDto
            {
                Id            = s.Id,
                OrderNumber   = s.OrderNumber,
                CustomerName  = s.CustomerName,
                CustomerPhone = s.CustomerPhone,
                TotalAmount   = s.TotalAmount,
                Discount      = s.Discount,
                TaxAmount     = s.TaxAmount,
                NetAmount     = s.NetAmount,
                PaymentMode   = s.PaymentMode,
                CashierName   = string.IsNullOrWhiteSpace(s.Cashier.FullName) ? "Admin" : s.Cashier.FullName,
                CreatedAt     = s.CreatedAt,
                Items         = s.Items.Select(i => new SalesOrderItemDto
                {
                    ProductId   = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity    = i.Quantity,
                    UnitPrice   = i.UnitPrice,
                    LineTotal   = i.LineTotal
                }).ToList()
            })
            .ToListAsync();

        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _db.SalesOrders
            .Include(s => s.Cashier)
            .Include(s => s.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(s => s.Id == id);

        return order is null ? NotFound() : Ok(order);
    }
}
