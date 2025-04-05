using EventBus.Base.Events;
using Shared.Events.DTOs.ShelfProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events.Events
{
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
}
