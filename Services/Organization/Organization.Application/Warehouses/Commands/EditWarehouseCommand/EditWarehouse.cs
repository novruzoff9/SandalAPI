
namespace Organization.Application.Warehouses.Commands.EditWarehouseCommand;

public record EditWarehouse(string Id, string Name,
string? GoogleMaps,
string City,
string State,
string Street,
string ZipCode,
string CompanyID) : IRequest<bool>;

public class EditWarehouseCommandHandler : IRequestHandler<EditWarehouse, bool>
{
    private readonly IApplicationDbContext _context;

    public EditWarehouseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(EditWarehouse request, CancellationToken cancellationToken)
    {
        Guard.Against.NotFound(request.Id, nameof(request));

        var warehouse = await _context.Warehouses.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (warehouse == null) { return false; }

        warehouse = new Warehouse
        {
            Name = request.Name,
            GoogleMaps = request.GoogleMaps,
            City = request.City,
            State = request.State,
            Street = request.Street,
            ZipCode = request.ZipCode,
            CompanyID = request.CompanyID
        };

        _context.Warehouses.Update(warehouse);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

