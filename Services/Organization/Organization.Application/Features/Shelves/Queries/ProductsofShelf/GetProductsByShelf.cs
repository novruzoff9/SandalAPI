using MediatR;

namespace Organization.Application.Features.Shelves.Queries.ProductsofShelf;

public record GetProductsByShelf(string id) : IRequest<List<ShelfProduct>>;

public class GetProductsByShelfHandler : IRequestHandler<GetProductsByShelf, List<ShelfProduct>>
{

    private readonly IApplicationDbContext _context;

    public GetProductsByShelfHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ShelfProduct>> Handle(GetProductsByShelf request, CancellationToken cancellationToken)
    {
        var products = await _context.ShelfProducts
            .Include(x => x.Product).Where(x => x.ShelfID == request.id).ToListAsync();
        foreach (var product in products)
        {
            product.Product.ShelfProducts = null;
            product.Shelf = null;
        }
        return products;
    }
}
