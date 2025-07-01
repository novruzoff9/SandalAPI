using Organization.Application.Common.Models.Shelf;

namespace Organization.Application.Shelves.Queries.GetShelvesQuery;

public record GetShelvesByWarehouse(string Id) : IRequest<List<ShelfDTO>>;

public class GetShelvesByWarehouseHandler : IRequestHandler<GetShelvesByWarehouse, List<ShelfDTO>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetShelvesByWarehouseHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<List<ShelfDTO>> Handle(GetShelvesByWarehouse request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(GetShelvesByWarehouse));
        var shelves = await _context.Shelves
            .Where(x => x.WarehouseID == request.Id)
            .Include(x => x.ShelfProducts)
            .ToListAsync(cancellationToken);
        if (shelves == null || !shelves.Any())
        {
            throw new Shared.Exceptions.NotFoundException($"No shelves found for warehouse.");
        }
        var shelvesDto = _mapper.Map<List<ShelfDTO>>(shelves);
        return shelvesDto;
    }
}
