namespace Organization.Application.Features.Shelves.Commands.CreateShelfCommand;

public record CreateShelf(string Code) : IRequest<bool>;

public class CreateShelfCommandHandler : IRequestHandler<CreateShelf, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;

    public CreateShelfCommandHandler(IApplicationDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<bool> Handle(CreateShelf request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(CreateShelf));

        string warehouseId = _sharedIdentityService.GetWarehouseId;

        var shelf = new Shelf(request.Code, warehouseId);

        await _context.Shelves.AddAsync(shelf, cancellationToken);

        var success = await _context.SaveChangesAsync(cancellationToken) > 0;

        return success;
    }
}

