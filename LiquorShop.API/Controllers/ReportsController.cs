using LiquorShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiquorShop.API.Controllers;

/// <summary>Sales, purchase, and low-stock reports.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reports;
    public ReportsController(IReportService reports) => _reports = reports;

    /// <summary>Daily sales summary for a date range.</summary>
    [HttpGet("sales-summary")]
    public async Task<IActionResult> SalesSummary([FromQuery] DateTime from, [FromQuery] DateTime to) =>
        Ok(await _reports.GetSalesSummaryAsync(from, to));

    /// <summary>Daily purchase summary for a date range.</summary>
    [HttpGet("purchase-summary")]
    public async Task<IActionResult> PurchaseSummary([FromQuery] DateTime from, [FromQuery] DateTime to) =>
        Ok(await _reports.GetPurchaseSummaryAsync(from, to));

    /// <summary>Products at or below reorder level.</summary>
    [HttpGet("low-stock")]
    public async Task<IActionResult> LowStock() =>
        Ok(await _reports.GetLowStockAsync());

    /// <summary>Detailed product purchase report grouped by date and product.</summary>
    [HttpGet("product-purchases")]
    public async Task<IActionResult> ProductPurchases([FromQuery] DateTime from, [FromQuery] DateTime to) =>
        Ok(await _reports.GetProductPurchasesAsync(from, to));

    /// <summary>Daily product sales report grouped by date and product.</summary>
    [HttpGet("daily-product-sales")]
    public async Task<IActionResult> DailyProductSales([FromQuery] DateTime from, [FromQuery] DateTime to) =>
        Ok(await _reports.GetDailyProductSalesAsync(from, to));

    /// <summary>Best selling products report for date range.</summary>
    [HttpGet("best-selling")]
    public async Task<IActionResult> BestSellingProducts([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] int top = 20) =>
        Ok(await _reports.GetBestSellingProductsAsync(from, to, top));
}
