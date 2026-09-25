using LiquorShop.API.DTOs;

namespace LiquorShop.API.Services.Interfaces;

public interface IOcrService
{
    Task<OcrResultDto>        ScanInvoiceAsync(IFormFile file);
    Task<AutoConfirmResultDto> ScanAndConfirmAsync(IFormFile file);
}
