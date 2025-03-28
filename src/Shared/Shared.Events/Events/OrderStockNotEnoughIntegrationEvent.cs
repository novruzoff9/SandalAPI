using EventBus.Base.Events;

namespace Shared.Events.Events;

public class OrderStockNotEnoughIntegrationEvent : IntegrationEvent
{
    public string OrderId { get; private set; }
    public Dictionary<string, int> NotEnoughProducts { get; private set; }
    public OrderStockNotEnoughIntegrationEvent(string orderId, Dictionary<string, int> notEnoughProducts)
    {
        OrderId = orderId;
        NotEnoughProducts = notEnoughProducts;
    }
}
