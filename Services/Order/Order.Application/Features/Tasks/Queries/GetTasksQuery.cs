namespace Order.Application.Features.Tasks.Queries;

public record GetTasksQuery : IRequest<List<OrderShowDto>>;

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, List<OrderShowDto>>
{
    private readonly IMapper _mapper;
    private readonly IOrderDbContext _context;
    private readonly ISharedIdentityService _identityService;
    public GetTasksQueryHandler(IOrderDbContext context, ISharedIdentityService identityService, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
        _identityService = identityService;
    }
    public async Task<List<OrderShowDto>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        string warehouseId = _identityService.GetWarehouseId;
        if (string.IsNullOrEmpty(warehouseId))
        {
            throw new ArgumentNullException("Warehouse Id cannot be null");
        }
        var orders = await _context.Orders
            .Include(x => x.Products)
            .Include(x => x.Status)
            .Where(x => x.WarehouseId == warehouseId && 
            x.Status == OrderStatus.StockConfirmed).ToListAsync(cancellationToken);

        var ordersDto = _mapper.Map<List<OrderShowDto>>(orders);
        return ordersDto;
    }
}
