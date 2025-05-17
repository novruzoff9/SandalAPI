using EventBus.Base.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.IntegratonEvents.Handlers;

public class OrderStockNotEnoughIntegrationEventHandler : IIntegrationEventHandler<OrderStockNotEnoughIntegrationEvent>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public OrderStockNotEnoughIntegrationEventHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task Handle(OrderStockNotEnoughIntegrationEvent @event, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IOrderDbContext>();
        var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == @event.OrderId);
        if (order is not null)
        {
            string orderNote = $"Not enough products in the warehouse: {string.Join(',', @event.NotEnoughProducts.Keys)}";
            order.UpdateNote(orderNote);
            order.UpdateStatus(OrderStatus.Cancelled);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
