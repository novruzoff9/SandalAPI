namespace Order.Application.Features.Orders.Queries.GetOrdersQuery;

public record GetOrdersQuery : IRequest<List<Domain.Entities.Order>>;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<Domain.Entities.Order>>
{
    private readonly IOrderDbContext _context;
    private readonly ISharedIdentityService _identityService;

    public GetOrdersQueryHandler(IOrderDbContext context, ISharedIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<List<Domain.Entities.Order>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        string companyId = _identityService.GetCompanyId;
        var orders = await _context.Orders.Where(x => x.CompanyId == companyId)
            .ToListAsync(cancellationToken);
        return orders;
    }
}

