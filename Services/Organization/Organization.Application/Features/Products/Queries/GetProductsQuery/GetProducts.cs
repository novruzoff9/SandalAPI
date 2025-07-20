using System.Linq.Expressions;

namespace Organization.Application.Features.Products.Queries.GetProductsQuery;

public record GetProducts(Expression<Func<Product, bool>> filter = null) : IRequest<List<Product>>;

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
        var productsQuery = _context.Products.Where(x => x.CompanyId == companyId).AsQueryable();
        if (request.filter != null)
        {
            productsQuery = productsQuery.Where(request.filter);
        }
        var products = await productsQuery.ToListAsync(cancellationToken);

        return products;
    }
}