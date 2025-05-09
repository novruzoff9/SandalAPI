using AutoMapper;
using Organization.Application.DTOs.Warehouse;

namespace Organization.Application.Warehouses.Queries.GetWarehousesQuery;

public record GetWarehousesByCompany(string companyId) : IRequest<List<WarehouseDto>>;

public class GetWarehousesOfCompanyHandler : IRequestHandler<GetWarehousesByCompany, List<WarehouseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetWarehousesOfCompanyHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<List<WarehouseDto>> Handle(GetWarehousesByCompany request, CancellationToken cancellationToken)
    {
        List<WarehouseDto> warehousesDto = new List<WarehouseDto>();
        Guard.Against.Null(request, nameof(GetWarehousesByCompany));
        var warehouses = await _context.Warehouses
            .Include(x => x.Shelves)
            .Where(x => x.CompanyID == request.companyId)
            .ToListAsync(cancellationToken);

        foreach (var warehouse in warehouses)
        {
            var warehouseDto = _mapper.Map<WarehouseDto>(warehouse);
            warehousesDto.Add(warehouseDto);
        }
        return warehousesDto;
    }
}
