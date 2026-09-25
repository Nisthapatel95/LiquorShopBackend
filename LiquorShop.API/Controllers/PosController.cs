using LiquorShop.API.Data;
using LiquorShop.API.Data.Entities;
using LiquorShop.API.DTOs;
using LiquorShop.API.Services;
using LiquorShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LiquorShop.API.Controllers;

/// <summary>Point-of-Sale checkout endpoint.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PosController : ControllerBase
{
    private readonly AppDbContext  _db;
    private readonly IStockService _stock;
    private readonly AuditService  _audit;

    public PosController(AppDbContext db, IStockService stock, AuditService audit)
    {
        _db    = db;
        _stock = stock;
        _audit = audit;
    }

    private int CurrentUserId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    /// <summary>
    /// Process a customer checkout: validates stock, creates the sales order,
    /// deducts inventory, and returns the bill.
    /// </summary>
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CreateSalesOrderDto dto)
    {
        // Pre-validate stock for all items
        foreach (var item in dto.Items)
        {
            var product = await _db.Products.FindAsync(item.ProductId);
            if (product is null)
                return BadRequest(new { message = $"Product {item.ProductId} not found." });
            if (product.CurrentStock < item.Quantity)
                return BadRequest(new { message = $"Insufficient stock for '{product.Name}'. Available: {product.CurrentStock}" });
        }

        var order = new SalesOrder
        {
            OrderNumber   = $"SO-{DateTime.UtcNow:yyyyMMddHHmmss}",
            CashierId     = dto.CashierId,
            CustomerName  = dto.CustomerName ?? string.Empty,
            CustomerPhone = dto.CustomerPhone ?? string.Empty,
            Discount      = dto.Discount,
            TaxAmount     = dto.TaxAmount,
            PaymentMode   = dto.PaymentMode
        };

        foreach (var item in dto.Items)
        {
            var lineTotal = item.Quantity * item.UnitPrice;
            order.Items.Add(new SalesOrderItem
            {
                ProductId = item.ProductId,
                Quantity  = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal  = lineTotal
            });
            order.TotalAmount += lineTotal;
        }

        order.NetAmount = order.TotalAmount - order.Discount + order.TaxAmount;

        _db.SalesOrders.Add(order);
        await _db.SaveChangesAsync();

        // Deduct stock for each item
        foreach (var item in order.Items)
            await _stock.DeductStockAsync(item.ProductId, item.Quantity, order.Id, $"Sale Order #{order.OrderNumber}");

        await _audit.LogAsync(CurrentUserId, "Create", "SalesOrder", order.Id, "",
            $"Order={order.OrderNumber},Net={order.NetAmount},Customer={order.CustomerName},Payment={order.PaymentMode}");

        var cashierUser = await _db.Users.FindAsync(order.CashierId);
        var finalCashierName = !string.IsNullOrWhiteSpace(dto.CashierName)
            ? dto.CashierName.Trim()
            : (cashierUser?.FullName ?? "Admin");

        return Ok(new SalesOrderDto
        {
            Id            = order.Id,
            OrderNumber   = order.OrderNumber,
            CustomerName  = order.CustomerName,
            CustomerPhone = order.CustomerPhone,
            TotalAmount   = order.TotalAmount,
            Discount      = order.Discount,
            TaxAmount     = order.TaxAmount,
            NetAmount     = order.NetAmount,
            PaymentMode   = order.PaymentMode,
            CashierName   = finalCashierName,
            CreatedAt     = order.CreatedAt
        });
    }
}
