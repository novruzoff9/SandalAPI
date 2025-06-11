using Shared.DTOs.ShelfProduct;

namespace Shared.Events;

public class OrderStockConfirmedIntegrationEvent : IntegrationEvent
{
    public OrderStockConfirmedIntegrationEvent(string orderId, List<ShelfProductDTO> shelfProducts)
    {
        OrderId = orderId;
        ShelfProducts = shelfProducts;
    }

    public string OrderId { get; set; }
    public List<ShelfProductDTO> ShelfProducts { get; set; }

}
