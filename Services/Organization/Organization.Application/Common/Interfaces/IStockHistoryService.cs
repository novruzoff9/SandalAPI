using Organization.Application.Common.Models.StockHistory;
using Organization.Domain.Enums;

namespace Organization.Application.Common.Interfaces;

public interface IStockHistoryService
{
    Task AddStockHistoryAsync(string companyId, string productId, int quantity, StockActionType actionType);
    Task<List<StockHistoryDto>> GetStockHistoriesByCompanyAsync();
    Task<List<StockHistoryDto>> GetStockHistoriesByProductAsync(string productId);
}
