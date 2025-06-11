namespace Shared.Events;

public class ProductStockBelowMinimumIntegrationEvent : IntegrationEvent
{
    public string ProductId { get; }
    public int MinimumStockLevel { get; }
    public int CurrentStockLevel { get; }
    public ProductStockBelowMinimumIntegrationEvent(string productId, int minimumStockLevel, int currentStockLevel)
    {
        ProductId = productId;
        MinimumStockLevel = minimumStockLevel;
        CurrentStockLevel = currentStockLevel;
    }
}
