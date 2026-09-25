using LiquorShop.API.DTOs;
using LiquorShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiquorShop.API.Controllers;

/// <summary>OCR invoice scanning endpoint.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class OcrController : ControllerBase
{
    private readonly IOcrService _ocr;
    public OcrController(IOcrService ocr) => _ocr = ocr;

    /// <summary>
    /// Upload a supplier invoice image (jpg/png/pdf).
    /// Returns extracted line items for admin verification.
    /// </summary>
    [HttpPost("scan")]
    [RequestSizeLimit(20 * 1024 * 1024)]
    public async Task<IActionResult> Scan(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No file uploaded." });

        var result = await _ocr.ScanInvoiceAsync(file);
        return Ok(result);
    }

    /// <summary>
    /// FULLY AUTOMATIC: Upload invoice → OCR → parse → create supplier & products → add stock.
    /// No manual verification required. Returns a summary of everything that was created.
    /// </summary>
    [HttpPost("scan-and-confirm")]
    [RequestSizeLimit(20 * 1024 * 1024)]
    public async Task<IActionResult> ScanAndConfirm(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No file uploaded." });

        var result = await _ocr.ScanAndConfirmAsync(file);

        // Always return 200 — the frontend reads result.success to decide what to show
        return Ok(result);
    }
}
