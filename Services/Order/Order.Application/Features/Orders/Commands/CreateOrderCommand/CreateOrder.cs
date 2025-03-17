using Order.Application.Common.DTOs.Address;
using Order.Application.DTOs.Order;
using Order.Domain.ValueObjects;

namespace Order.Application.Features.Orders.Commands.CreateOrderCommand;

public record CreateOrderCommand(string warehouseId, string warehouseName,
    string customerId, List<OrderItemDto> orderItems, AddressDto Address) : IRequest<bool>;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
{
    private readonly IOrderDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;

    public CreateOrderCommandHandler(IOrderDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
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

        var newOrder = new Domain.Entities.Order(userId,newAddress, companyId, request.warehouseId, request.warehouseName);

        foreach (var item in request.orderItems)
        {
            newOrder.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.ImageUrl);
        }

        await _context.Orders.AddAsync(newOrder, cancellationToken);

        var success = await _context.SaveChangesAsync(cancellationToken) > 0;

        return success;
    }
}

