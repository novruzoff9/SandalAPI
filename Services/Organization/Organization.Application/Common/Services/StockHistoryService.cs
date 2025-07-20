using Organization.Application.Common.Models.StockHistory;
using Organization.Domain.Enums;

namespace Organization.Application.Common.Services;

public class StockHistoryService : IStockHistoryService
{
    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _identityService;

    public StockHistoryService(IApplicationDbContext context, ISharedIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task AddStockHistoryAsync(string companyId, string productId, int quantity, StockActionType actionType)
    {
        var stockHistory = new StockHistory(companyId, productId, quantity, actionType);
        await _context.StockHistories.AddAsync(stockHistory);
    }

    public Task<List<StockHistoryDto>> GetStockHistoriesByCompanyAsync()
    {
        var companyId = _identityService.GetCompanyId;
        var stockHistories = _context.StockHistories
            .Include(sh=>sh.Product)
            .Where(sh => sh.CompanyId == companyId)
            .Select(sh => new StockHistoryDto
            {
                CompanyId = sh.CompanyId,
                ProductId = sh.ProductId,
                ProductName = sh.Product!.Name,
                Quantity = sh.QuantityChanged,
                ActionType = sh.ActionType,
                CreatedAt = sh.CreatedAt
            })
            .OrderByDescending(sh => sh.CreatedAt)
            .ToListAsync();
        return stockHistories;
    }

    public Task<List<StockHistoryDto>> GetStockHistoriesByProductAsync(string productId)
    {
        var stockHistories = _context.StockHistories
            .Where(sh => sh.ProductId == productId)
            .Select(sh => new StockHistoryDto
            {
                CompanyId = sh.CompanyId,
                ProductId = sh.ProductId,
                ProductName = sh.Product!.Name,
                Quantity = sh.QuantityChanged,
                ActionType = sh.ActionType,
                CreatedAt = sh.CreatedAt
            })
            .ToListAsync();
        return stockHistories;
    }
}
