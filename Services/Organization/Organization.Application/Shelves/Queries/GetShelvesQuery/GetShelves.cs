using Organization.Application.Common.Models.Shelf;

namespace Organization.Application.Shelves.Queries.GetShelvesQuery;

public record GetShelves : IRequest<List<ShelfDTO>>;

public class GetShelvesQueryHandler : IRequestHandler<GetShelves, List<ShelfDTO>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;


    public GetShelvesQueryHandler(IApplicationDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<List<ShelfDTO>> Handle(GetShelves request, CancellationToken cancellationToken)
    {
        string warehouseId = _sharedIdentityService.GetWarehouseId;
        var shelves = await _context.Shelves
            .Include(x => x.ShelfProducts)
            .Select(x => new ShelfDTO
            {
                Id = x.Id,
                Code = x.Code,
                WarehouseID = x.WarehouseID,
                ItemsCount = x.ShelfProducts.Sum(y=>y.Quantity)
            })
            .Where(x => x.WarehouseID == warehouseId)
            .ToListAsync(cancellationToken);

        //var shelves = await _context.Shelves
        //    .Where(x=>x.WarehouseID == warehouseId)
        //    .ToListAsync(cancellationToken);
        return shelves;
    }
}

