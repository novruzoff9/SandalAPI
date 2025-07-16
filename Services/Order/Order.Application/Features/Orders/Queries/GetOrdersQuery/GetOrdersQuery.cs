namespace Order.Application.Features.Orders.Queries.GetOrdersQuery;

public record GetOrdersQuery(Expression<Func<Order.Domain.Entities.Order, bool>>? filter = null) : IRequest<List<OrderShowDto>>;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<OrderShowDto>>
{
    private readonly IMapper _mapper;
    private readonly IOrderDbContext _context;
    private readonly ICustomerService _customerService;
    private readonly ISharedIdentityService _identityService;

    public GetOrdersQueryHandler(IOrderDbContext context, ISharedIdentityService identityService, IMapper mapper, ICustomerService customerService)
    {
        _mapper = mapper;
        _context = context;
        _identityService = identityService;
        _customerService = customerService;
    }

    public async Task<List<OrderShowDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        string companyId = _identityService.GetCompanyId;
        var ordersQuery = _context.Orders.Where(x => x.CompanyId == companyId)
            .Include(x => x.Products)
            .Include(x => x.Status)
            .AsQueryable();

        if (request.filter != null)
        {
            ordersQuery = ordersQuery.Where(request.filter).AsQueryable();
        }
        var orders = await ordersQuery.ToListAsync(cancellationToken);

        var ordersDto = _mapper.Map<List<OrderShowDto>>(orders);
        var tasks = ordersDto.Select(async order =>
        {
            order.Customer = await _customerService.GetCustomerFullNameAsync(order.Customer);
        }).ToList();

        await Task.WhenAll(tasks);

        return ordersDto;
    }
}

