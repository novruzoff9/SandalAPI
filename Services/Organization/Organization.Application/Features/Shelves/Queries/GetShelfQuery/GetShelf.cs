namespace Organization.Application.Features.Shelves.Queries.GetShelfQuery;

public record GetShelf(string Id) : IRequest<Shelf>;

public class GetShelfQueryHandler : IRequestHandler<GetShelf, Shelf>
{
    private readonly IApplicationDbContext _context;

    public GetShelfQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Shelf> Handle(GetShelf request, CancellationToken cancellationToken)
    {
        var shelf = await _context.Shelves
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (shelf == null)
        {
            throw new Shared.Exceptions.NotFoundException($"Shelf with ID {request.Id} not found.");
        }
        return shelf;
    }
}

