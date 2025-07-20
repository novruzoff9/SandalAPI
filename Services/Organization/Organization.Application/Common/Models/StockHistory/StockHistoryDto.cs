using Organization.Domain.Enums;

namespace Organization.Application.Common.Models.StockHistory;

public class StockHistoryDto
{
    public string CompanyId { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public StockActionType ActionType { get; set; }
    public string ActionTypeText => ActionType.ToString();
    public DateTime CreatedAt { get; set; }
}
