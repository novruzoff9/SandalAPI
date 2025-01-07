
namespace Organization.Application.Products.Queries.GetProductQuery;

public record GetProduct(string Id) : IRequest<Product>;

public class GetProductQueryHandler : IRequestHandler<GetProduct, Product>
{
    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;

    public GetProductQueryHandler(IApplicationDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<Product> Handle(GetProduct request, CancellationToken cancellationToken)
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == request.Id && x.CompanyID == companyId, cancellationToken);
        return product;
    }
}

