using AutoMapper;
using Organization.Application.DTOs.Warehouse;

namespace Organization.Application.Warehouses.Queries.GetWarehousesQuery;

public record GetWarehouses : IRequest<List<WarehouseDto>>;

public class GetWarehousesQueryHandler : IRequestHandler<GetWarehouses, List<WarehouseDto>>
{

    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;
    private readonly IMapper _mapper;

    public GetWarehousesQueryHandler(IApplicationDbContext context, ISharedIdentityService sharedIdentityService, IMapper mapper)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
        _mapper = mapper;
    }

    public async Task<List<WarehouseDto>> Handle(GetWarehouses request, CancellationToken cancellationToken)
    {
        List<WarehouseDto> warehousesDto = new List<WarehouseDto>();
        string company = _sharedIdentityService.GetCompanyId;

        var warehouses = await _context.Warehouses
            .Where(x => x.CompanyID == company)
            .Include(x => x.Shelves)
            .ToListAsync(cancellationToken);

        foreach (var warehouse in warehouses)
        {
            var warehouseDto = _mapper.Map<WarehouseDto>(warehouse);
            warehousesDto.Add(warehouseDto);
        }
        return warehousesDto;
    }
}

