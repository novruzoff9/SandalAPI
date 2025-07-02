using Organization.Application.DTOs.Warehouse;

namespace Organization.Application.Warehouses.Queries.GetWarehousesQuery;

public record GetWarehouses : IRequest<List<WarehouseDto>>;

public class GetWarehousesQueryHandler : IRequestHandler<GetWarehouses, List<WarehouseDto>>
{

    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;
    private readonly IIdentityGrpcClient _identityGrpcClient;
    private readonly ISharedIdentityService _sharedIdentityService;

    public GetWarehousesQueryHandler(IApplicationDbContext context, ISharedIdentityService sharedIdentityService, IMapper mapper, IIdentityGrpcClient identityGrpcClient)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
        _mapper = mapper;
        _identityGrpcClient = identityGrpcClient;
    }

    public async Task<List<WarehouseDto>> Handle(GetWarehouses request, CancellationToken cancellationToken)
    {
        List<WarehouseDto> warehousesDto = new List<WarehouseDto>();
        string company = _sharedIdentityService.GetCompanyId;

        var warehouses = await _context.Warehouses
            .Where(x => x.CompanyID == company)
            .Include(x => x.Shelves)
            .ThenInclude(s => s.ShelfProducts)
            .ToListAsync(cancellationToken);

        

        foreach (var warehouse in warehouses)
        {
            decimal emptySheleves = warehouse.Shelves
            .Where(x => x.ShelfProducts.Sum(x => x.Quantity) == 0).Count();
            decimal totalShelves = warehouse.Shelves.Count;
            decimal fullShelves = totalShelves - emptySheleves;
            decimal occupancyRate = fullShelves / totalShelves;

            WarehouseDto warehouseDto = _mapper.Map<WarehouseDto>(warehouse);
            warehouseDto.OccupancyRate = occupancyRate;
            warehouseDto.UsedShelves = (int)fullShelves;

            //TODO: Isci sayini gostermek qalib
            warehouseDto.EmployeeCount = await _identityGrpcClient.GetEmployeeCountOfWarehouseAsync(warehouse.Id);
            warehousesDto.Add(warehouseDto);
        }
        return warehousesDto;
    }
}

