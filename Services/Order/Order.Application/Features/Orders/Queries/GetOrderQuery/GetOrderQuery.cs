namespace Order.Application.Features.Orders.Queries.GetOrderQuery;

public record GetOrderQuery(string Id) : IRequest<Domain.Entities.Order>;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, Domain.Entities.Order>
{
    private readonly IOrderDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;

    public GetOrderQueryHandler(IOrderDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<Domain.Entities.Order> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        var order = await _context.Orders
            .Include(x => x.Products)
            .Include(x=>x.Status)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (order == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Order), request.Id);
        }

        foreach (var item in order.Products)
        {
            item.Order = null;
        }
        return order;
    }
}

