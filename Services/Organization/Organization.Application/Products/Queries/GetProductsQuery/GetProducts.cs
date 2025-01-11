
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
        var products = await _context.Products.Where(x=>x.CompanyId == companyId).ToListAsync(cancellationToken);

        return products;
    }
}

