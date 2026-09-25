using LiquorShop.API.DTOs;

namespace LiquorShop.API.Services.Interfaces;

public interface IStockService
{
    Task AddStockAsync(int productId, int quantity, int referenceId, string note);
    Task DeductStockAsync(int productId, int quantity, int referenceId, string note);
    Task AdjustStockAsync(int productId, int quantity, string note);
    Task<List<StockReportDto>> GetCurrentStockAsync();
}
