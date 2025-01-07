namespace Organization.Application.Shelves.Queries.GetShelvesQuery;

public record GetShelvesByWarehouse(string Id) : IRequest<List<Shelf>>;

public class GetShelvesByWarehouseHandler : IRequestHandler<GetShelvesByWarehouse, List<Shelf>>
{
    private readonly IApplicationDbContext _context;
    public GetShelvesByWarehouseHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<Shelf>> Handle(GetShelvesByWarehouse request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(GetShelvesByWarehouse));
        var shelves = await _context.Shelves
            .Where(x => x.WarehouseID == request.Id)
            .ToListAsync(cancellationToken);
        return shelves;
    }
}
