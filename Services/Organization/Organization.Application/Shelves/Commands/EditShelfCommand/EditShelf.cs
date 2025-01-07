
namespace Organization.Application.Shelves.Commands.EditShelfCommand;

public record EditShelf(string Id, string Code, string WarehouseID) : IRequest<bool>;

public class EditShelfCommandHandler : IRequestHandler<EditShelf, bool>
{
    private readonly IApplicationDbContext _context;

    public EditShelfCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(EditShelf request, CancellationToken cancellationToken)
    {
        Guard.Against.NotFound(request.Id, nameof(request));

        var shelf = await _context.Shelves.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (shelf == null) { return false; }

        shelf = new Shelf
        {
            Code = request.Code,
            WarehouseID = request.WarehouseID
        };

        _context.Shelves.Update(shelf);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

