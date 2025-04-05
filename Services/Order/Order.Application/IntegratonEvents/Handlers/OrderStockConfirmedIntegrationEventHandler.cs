using EventBus.Base.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Shared.Events.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.IntegratonEvents.Handlers;

public class OrderStockConfirmedIntegrationEventHandler : IIntegrationEventHandler<OrderStockConfirmedIntegrationEvent>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public OrderStockConfirmedIntegrationEventHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task Handle(OrderStockConfirmedIntegrationEvent @event, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IOrderDbContext>();
        var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == @event.OrderId);
        if(order is not null)
        {
            string orderNote = $"Products are removed from the warehouse: {string.Join(',', @event.ShelfProducts.Select(x => x.ProductName))}";
            order.UpdateNote(orderNote);
            order.UpdateStatus(OrderStatus.StockConfirmed);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
