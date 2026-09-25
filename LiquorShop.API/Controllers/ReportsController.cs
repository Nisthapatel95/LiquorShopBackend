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
}
