namespace Organization.Application.Warehouses.Commands.DeleteWarehouseCommand;

public record DeleteWarehouse(string Id) : IRequest<bool>;

public class DeleteWarehouseCommandHandler : IRequestHandler<DeleteWarehouse, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;

    public DeleteWarehouseCommandHandler(IApplicationDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<bool> Handle(DeleteWarehouse request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request.Id, nameof(request.Id));

        string companyId = _sharedIdentityService.GetCompanyId;

        var warehouse = await _context.Warehouses.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (warehouse == null) { return false; }
        if (warehouse.CompanyID != companyId) { return false; }
        _context.Warehouses.Remove(warehouse);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

