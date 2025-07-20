
using AutoMapper;
using Organization.Application.DTOs.Warehouse;

namespace Organization.Application.Features.Warehouses.Queries.GetWarehouseQuery;

public record GetWarehouse(string Id) : IRequest<WarehouseDto>;

public class GetWarehouseQueryHandler : IRequestHandler<GetWarehouse, WarehouseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetWarehouseQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<WarehouseDto> Handle(GetWarehouse request, CancellationToken cancellationToken)
    {
        var warehouse = await _context.Warehouses
            .Include(x => x.Shelves)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        decimal emptySheleves = warehouse.Shelves
            .Where(x => x.ShelfProducts.Sum(x => x.Quantity) == 0).Count();
        decimal totalShelves = warehouse.Shelves.Count;
        decimal fullShelves = totalShelves - emptySheleves;
        decimal occupancyRate = fullShelves / totalShelves;

        WarehouseDto warehouseDto = _mapper.Map<WarehouseDto>(warehouse);
        warehouseDto.OccupancyRate = occupancyRate;
        return warehouseDto;
    }
}

