using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events.Events;

public class OrderCreatedIntegrationEvent : IntegrationEvent
{
    public string OrderId { get; private set; }
    public string CustomerId { get; private set; }
    public string WarehouseId { get; private set; }
    public List<OrderItemDto> Items { get; private set; }

    public OrderCreatedIntegrationEvent(string orderId, string customerId, string warehouseId, List<OrderItemDto> items)
    {
        OrderId = orderId;
        CustomerId = customerId;
        WarehouseId = warehouseId;
        Items = items;
    }
}

public class OrderItemDto
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
}
