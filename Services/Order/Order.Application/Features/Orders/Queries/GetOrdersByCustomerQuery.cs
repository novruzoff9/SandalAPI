namespace Order.Application.Features.Orders.Queries;

public record GetOrdersByCustomerQuery(string id) : IRequest<List<OrderShowDto>>;

public class GetOrdersByCustomerQueryHandler : IRequestHandler<GetOrdersByCustomerQuery, List<OrderShowDto>>
{
    private readonly IMapper _mapper;
    private readonly IOrderDbContext _context;
    private readonly ICustomerService _customerService;
    private readonly ISharedIdentityService _sharedIdentityService;
    public GetOrdersByCustomerQueryHandler(IOrderDbContext context, ISharedIdentityService sharedIdentityService, IMapper mapper, ICustomerService customerService)
    {
        _mapper = mapper;
        _context = context;
        _customerService = customerService;
        _sharedIdentityService = sharedIdentityService;
    }
    public async Task<List<OrderShowDto>> Handle(GetOrdersByCustomerQuery request, CancellationToken cancellationToken)
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        var orders = await _context.Orders
            .Include(x => x.Products)
            .Include(x => x.Status)
            .Where(x => x.CompanyId == companyId && x.CustomerId == request.id)
            .ToListAsync(cancellationToken);

        var orderDtos = _mapper.Map<List<OrderShowDto>>(orders);
        foreach (var item in orderDtos)
        {
            item.Customer = await _customerService.GetCustomerFullNameAsync(item.Customer);
        }

        return orderDtos;
    }
}
