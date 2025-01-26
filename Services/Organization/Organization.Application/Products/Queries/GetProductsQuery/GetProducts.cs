
using Shared.ResultTypes;

namespace Organization.Application.Products.Queries.GetProductsQuery;

public record GetProducts : IRequest<List<Product>>;

public class GetProductsQueryHandler : IRequestHandler<GetProducts, List<Product>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;

    public GetProductsQueryHandler(IApplicationDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<List<Product>> Handle(GetProducts request, CancellationToken cancellationToken)
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        var products = await _context.Products.Where(x => x.CompanyId == companyId).ToListAsync(cancellationToken);

        return products;
    }
}


public record GetProductsWithOrder(DateTime StartDate, DateTime EndDate) : IRequest<List<Product>>;

public class GetProductsWithOrderHandler : IRequestHandler<GetProductsWithOrder, List<Product>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;

    public GetProductsWithOrderHandler(IApplicationDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<List<Product>> Handle(GetProductsWithOrder request, CancellationToken cancellationToken)
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        var products = await _context.Products
            .Where(x => x.CompanyId == companyId)
            .Include(x => x.OrderItems)
            .ThenInclude(x => x.Order)
            .ToListAsync(cancellationToken);

        foreach (var product in products)
        {
            product.OrderItems = product.OrderItems
                .Where(oi => oi.Order.Opened >= request.StartDate && oi.Order.Opened <= request.EndDate)
                .ToList();
        }

        return products;
    }
}

