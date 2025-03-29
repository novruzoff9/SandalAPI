
namespace Organization.Application.Customers.Queries.GetCustomersQuery;

public record GetCustomersQuery : IRequest<List<Customer>>;

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, List<Customer>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;

    public GetCustomersQueryHandler(IApplicationDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<List<Customer>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        var customers = await _context.Customers
            .Where(x=>x.CompanyId == companyId).ToListAsync(cancellationToken);
        return customers;
    }
}

