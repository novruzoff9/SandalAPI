namespace Organization.Application.Warehouses.Commands.CreateWarehouseCommand;

public record CreateWarehouse(string Name,
string? GoogleMaps,
string City,
string State,
string Street,
string ZipCode) : IRequest<bool>;

public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouse, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;

    public CreateWarehouseCommandHandler(IApplicationDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<bool> Handle(CreateWarehouse request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(CreateWarehouse));

        string companyId = _sharedIdentityService.GetCompanyId;

        var warehouse = new Warehouse
        (
            request.Name,
            request.GoogleMaps,
            request.City,
            request.State,
            request.Street,
            request.ZipCode,
            companyId
        );

        await _context.Warehouses.AddAsync(warehouse, cancellationToken);

        var success = await _context.SaveChangesAsync(cancellationToken) > 0;

        return success;
    }
}

