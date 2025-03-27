using EventBus.Base.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Organization.Application.Common.Services;
using Organization.Application.DTOs.Shelf;
using Shared.Events.Events;
using Shared.ResultTypes;

namespace Organization.Application.IntegrationEvent.Handlers;

public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    private readonly ILogger<OrderCreatedIntegrationEventHandler> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEventBus _eventBus;

    public OrderCreatedIntegrationEventHandler(ILogger<OrderCreatedIntegrationEventHandler> logger, IServiceScopeFactory scopeFactory, IEventBus eventBus)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _eventBus = eventBus;
    }

    public async Task Handle(OrderCreatedIntegrationEvent @event, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var shelfProductService = scope.ServiceProvider.GetRequiredService<ShelfProductService>();
        _logger.LogInformation($"OrderCreatedIntegrationEvent: {@event.OrderId}");
        var orderItems = @event.Items;
        Dictionary<string, int> notEnoughProducts = new();
        List<ShelfProductDTO> shelfProducts = new();
        foreach (var item in @event.Items)
        {
            ShelfProductDTO shelfProduct = await shelfProductService.GetShelfProduct(item.ProductId, item.Quantity);
            if (shelfProduct.ShelfCode is null)
            {
                notEnoughProducts.Add(shelfProduct.ProductName, item.Quantity);
                _logger.LogError($"Product {shelfProduct.ProductName} is not available in the warehouse");
            }
            else
            {
                //shelfProduct.Quantity -= item.Quantity;
                //_context.ShelfProducts.Update(shelfProduct);
                shelfProducts.Add(shelfProduct);
                _logger.LogInformation($"Product {shelfProduct.ProductName} is removed from the {shelfProduct.ShelfCode} shelf {shelfProduct.Quantity} count at warehouse");
            }
        }
        if(notEnoughProducts.Count > 0)
        {
            var notEnoughProductsEvent = new OrderStockNotEnoughIntegrationEvent(@event.OrderId, notEnoughProducts);
            _eventBus.Publish(notEnoughProductsEvent);
        }
        else
        {
            //TODO: Mehsullar esasinda anbarda kagiz cap olunmalidi ki, anbardar mehsullari yığışdırıla bilsin

        }
    }
}
