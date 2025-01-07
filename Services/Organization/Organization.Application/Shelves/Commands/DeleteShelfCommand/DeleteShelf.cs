
using Shared.Services;

namespace Organization.Application.Shelves.Commands.DeleteShelfCommand;

public record DeleteShelf(string Id) : IRequest<bool>;

public class DeleteShelfCommandHandler : IRequestHandler<DeleteShelf, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;

    public DeleteShelfCommandHandler(IApplicationDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<bool> Handle(DeleteShelf request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request.Id, nameof(request.Id));

        string warehouseId = _sharedIdentityService.GetWarehouseId;

        var shelf = await _context.Shelves.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (shelf == null) { return false; }
        if (shelf.WarehouseID != warehouseId) { return false; }
        _context.Shelves.Remove(shelf);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

