using AutoMapper;
using EventBus.Base.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Organization.Application.Common.Services;
using Organization.Application.DTOs.Shelf;
using Shared.Events;

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
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        _logger.LogInformation($"OrderCreatedIntegrationEvent: {@event.OrderId}");
        var orderItems = @event.Items;
        Dictionary<string, int> notEnoughProducts = new();
        List<ShelfProductDTO> shelfProducts = new();
        foreach (var item in @event.Items)
        {
            ShelfProductDTO shelfProductDto = await shelfProductService.GetShelfProduct(item.ProductId, item.Quantity);
            if (shelfProductDto.ShelfCode is null)
            {
                notEnoughProducts.Add(shelfProductDto.ProductName, item.Quantity);
                _logger.LogError($"Product {shelfProductDto.ProductName} is not available in the warehouse");
            }
            else
            {
                await shelfProductService.RemoveProductFromShelf(shelfProductDto.ProductId, item.Quantity, shelfProductDto.ShelfId, cancellationToken);

                shelfProducts.Add(shelfProductDto);
                _logger.LogInformation($"Product {shelfProductDto.ProductName} is removed from the {shelfProductDto.ShelfCode} shelf {shelfProductDto.Quantity} count at warehouse");
            }
        }
        if(notEnoughProducts.Count > 0)
        {
            var notEnoughProductsEvent = new OrderStockNotEnoughIntegrationEvent(@event.OrderId, notEnoughProducts);
            _eventBus.Publish(notEnoughProductsEvent);
        }
        else
        {
            var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

            var shelfProductsDto = mapper.Map<List<Shared.DTOs.ShelfProduct.ShelfProductDTO>>(shelfProducts);

            var stockConfirmedEvent = new OrderStockConfirmedIntegrationEvent(@event.OrderId, shelfProductsDto);

            _eventBus.Publish(stockConfirmedEvent);
        }
    }
}
