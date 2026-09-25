using LiquorShop.API.DTOs;

namespace LiquorShop.API.Services.Interfaces;

public interface IReportService
{
    Task<List<SalesSummaryDto>>    GetSalesSummaryAsync(DateTime from, DateTime to);
    Task<List<PurchaseSummaryDto>> GetPurchaseSummaryAsync(DateTime from, DateTime to);
    Task<List<LowStockDto>>        GetLowStockAsync();
}
