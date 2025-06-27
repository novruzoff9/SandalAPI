namespace Order.Application.Features.Orders.Queries.GetOrdersByWarehouseQuery;

public record GetOrdersByWarehouseQuery(string? warehouseId) : IRequest<List<OrderShowDto>>;

public class GetOrdersByWarehouseQueryHandler : IRequestHandler<GetOrdersByWarehouseQuery, List<OrderShowDto>>
{
    private readonly IMapper _maper;
    private readonly IOrderDbContext _context;
    private readonly ISharedIdentityService _identityService;
    public GetOrdersByWarehouseQueryHandler(IOrderDbContext context, ISharedIdentityService identityService, IMapper maper)
    {
        _maper = maper;
        _context = context;
        _identityService = identityService;
    }
    public async Task<List<OrderShowDto>> Handle(GetOrdersByWarehouseQuery request, CancellationToken cancellationToken)
    {
        string warehouseId = _identityService.GetWarehouseId;

        if (string.IsNullOrEmpty(request.warehouseId) && warehouseId == "N/A")
        {
            throw new ArgumentNullException("Warehouse Id cannot be null");
        }

        if (warehouseId == "N/A")
        {
            var ordersOfWarehouse = await _context.Orders
                .Include(x => x.Products)
                .Where(x => x.WarehouseId == request.warehouseId).ToListAsync(cancellationToken);
            var ordersOfWarehouseDto = _maper.Map<List<OrderShowDto>>(ordersOfWarehouse);
            return ordersOfWarehouseDto;
        }

        var orders = await _context.Orders
            .Include(x => x.Products)
            .Where(x => x.WarehouseId == warehouseId).ToListAsync(cancellationToken);
        var ordersDto = _maper.Map<List<OrderShowDto>>(orders);
        return ordersDto;
    }
}
