using Order.Application.Common.Services;
using Order.Application.DTOs.Order;

namespace Order.Application.Features.Orders.Queries.GetOrdersQuery;

public record GetOrdersQuery : IRequest<List<OrderShowDto>>;

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
        var orders = await _context.Orders.Where(x => x.CompanyId == companyId)
            .Include(x => x.Products)
            .Include(x => x.Status)
            .ToListAsync(cancellationToken);

        var ordersDto = _mapper.Map<List<OrderShowDto>>(orders);

        foreach (var orderDto in ordersDto)
        {
            orderDto.Customer = await _customerService.GetCustomerFullNameAsync(orderDto.Customer);
        }
        return ordersDto;
    }
}

