using Organization.Domain.Common;
using Organization.Domain.Enums;

namespace Organization.Domain.Entities;

public class StockHistory : BaseEntity
{
    public int QuantityChanged { get; init; }
    public StockActionType ActionType { get; init; }
    public DateTime CreatedAt { get; init; }

    // Hər ehtimala qarşı future integration üçün
    public string? OrderId { get; init; }

    public string CompanyId { get; set; }

    public string ProductId { get; init; }
    public Product? Product { get; init; }

    private StockHistory() { }

    public StockHistory(string companyId, string productId, int quantityChanged, StockActionType actionType, string? orderId = null)
    {
        Id = Guid.NewGuid().ToString();
        CompanyId = companyId;
        ProductId = productId;
        QuantityChanged = quantityChanged;
        ActionType = actionType;
        CreatedAt = DateTime.Now;
        OrderId = orderId;
    }
}
