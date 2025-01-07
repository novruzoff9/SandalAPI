namespace Organization.Application.Warehouses.Queries.GetWarehousesQuery;

public record GetWarehouses : IRequest<List<Warehouse>>;

public class GetWarehousesQueryHandler : IRequestHandler<GetWarehouses, List<Warehouse>>
{

    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;

    public GetWarehousesQueryHandler(IApplicationDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<List<Warehouse>> Handle(GetWarehouses request, CancellationToken cancellationToken)
    {
        string company = _sharedIdentityService.GetCompanyId;

        var warehouses = await _context.Warehouses.Where(x=>x.CompanyID == company).ToListAsync(cancellationToken);
        return warehouses;
    }
}

