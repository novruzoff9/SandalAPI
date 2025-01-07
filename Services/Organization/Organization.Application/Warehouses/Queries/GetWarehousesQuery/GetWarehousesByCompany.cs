namespace Organization.Application.Warehouses.Queries.GetWarehousesQuery;

public record GetWarehousesByCompany(string companyId) : IRequest<List<Warehouse>>;

public class GetWarehousesOfCompanyHandler : IRequestHandler<GetWarehousesByCompany, List<Warehouse>>
{
    private readonly IApplicationDbContext _context;
    public GetWarehousesOfCompanyHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<Warehouse>> Handle(GetWarehousesByCompany request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(GetWarehousesByCompany));
        var warehouses = await _context.Warehouses
            .Where(x => x.CompanyID == request.companyId)
            .ToListAsync(cancellationToken);
        return warehouses;
    }
}
