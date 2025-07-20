namespace Organization.Application.Features.Warehouses.Commands.EditWarehouseCommand;

public record EditWarehouse(string Id, string Name,
string? GoogleMaps,
string City,
string State,
string Street,
string ZipCode) : IRequest<bool>;

public class EditWarehouseCommandHandler : IRequestHandler<EditWarehouse, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;

    public EditWarehouseCommandHandler(IApplicationDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<bool> Handle(EditWarehouse request, CancellationToken cancellationToken)
    {
        Guard.Against.NotFound(request.Id, nameof(request));
        var companyId = _sharedIdentityService.GetCompanyId;

        var warehouse = await _context.Warehouses.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (warehouse == null) { return false; }

        if (warehouse.CompanyID != companyId)
        {
            return false;
        }

        warehouse.Update(
            request.Name,
            request.City,
            request.State,
            request.Street,
            request.ZipCode,
            request.GoogleMaps
        );

        _context.Warehouses.Update(warehouse);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

