using EventBus.Base.Abstraction;
using Order.Application.Common.DTOs.Address;
using Order.Application.DTOs.Order;
using Order.Domain.Entities;
using Order.Domain.ValueObjects;
using Shared.Events;

namespace Order.Application.Features.Orders.Commands.CreateOrderCommand;

public record CreateOrderCommand(string warehouseId, string warehouseName,
    string customerId, List<DTOs.Order.OrderItemDto> orderItems, AddressDto Address) : IRequest<bool>;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
{
    private readonly IOrderDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;
    private readonly IEventBus _eventBus;

    public CreateOrderCommandHandler(IOrderDbContext context, ISharedIdentityService sharedIdentityService, IEventBus eventBus)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(CreateOrderCommand));

        string companyId = _sharedIdentityService.GetCompanyId;
        string userId = _sharedIdentityService.GetUserId;
        var newAddress = new Address(
            city: request.Address.City,
            district: request.Address.District,
            street: request.Address.Street,
            zipCode: request.Address.ZipCode
        );

        var newOrder = new Domain.Entities.Order(userId,newAddress, companyId, request.warehouseId, request.warehouseName, request.customerId);

        foreach (var item in request.orderItems)
        {
            newOrder.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.ImageUrl, item.Quantity);
        }

        await _context.Orders.AddAsync(newOrder, cancellationToken);

        bool success = await _context.SaveChangesAsync(cancellationToken) > 0; 

        OrderCreatedIntegrationEvent orderCreatedEvent = new(
            newOrder.Id, request.customerId, request.warehouseId,
            newOrder.Products.Select(p => new Shared.Events.OrderItemDto
            {
                ProductId = p.ProductId,
                Quantity = p.Quantity
            }).ToList());

        _eventBus.Publish(orderCreatedEvent);

        return success;
    }
}

