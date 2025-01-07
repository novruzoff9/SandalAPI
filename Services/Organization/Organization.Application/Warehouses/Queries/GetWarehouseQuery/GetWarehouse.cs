
namespace Organization.Application.Warehouses.Queries.GetWarehouseQuery;

public record GetWarehouse(string Id) : IRequest<Warehouse>;

public class GetWarehouseQueryHandler : IRequestHandler<GetWarehouse, Warehouse>
{
    private readonly IApplicationDbContext _context;

    public GetWarehouseQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Warehouse> Handle(GetWarehouse request, CancellationToken cancellationToken)
    {
        var warehouse = await _context.Warehouses.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return warehouse;
    }
}

