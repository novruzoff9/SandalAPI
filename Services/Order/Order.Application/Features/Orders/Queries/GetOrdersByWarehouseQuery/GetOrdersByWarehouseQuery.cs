using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Queries.GetOrdersByWarehouseQuery;

public record GetOrdersByWarehouseQuery(string? warehouseId) : IRequest<List<Domain.Entities.Order>>;

public class GetOrdersByWarehouseQueryHandler : IRequestHandler<GetOrdersByWarehouseQuery, List<Domain.Entities.Order>>
{
    private readonly IOrderDbContext _context;
    private readonly ISharedIdentityService _identityService;
    public GetOrdersByWarehouseQueryHandler(IOrderDbContext context, ISharedIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }
    public async Task<List<Domain.Entities.Order>> Handle(GetOrdersByWarehouseQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.warehouseId))
        {
            throw new ArgumentNullException("Warehouse Id cannot be null");
        }
        string warehouseId = _identityService.GetWarehouseId;

        if (warehouseId == "N/A")
        {
            var ordersOfWarehouse = await _context.Orders
                .Where(x => x.WarehouseId == request.warehouseId).ToListAsync(cancellationToken);
            return ordersOfWarehouse;
        }

        var orders = await _context.Orders
            .Where(x => x.WarehouseId == warehouseId).ToListAsync(cancellationToken);
        return orders;
    }
}
