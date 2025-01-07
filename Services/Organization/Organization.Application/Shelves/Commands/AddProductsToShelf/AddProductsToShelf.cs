namespace Organization.Application.Shelves.Commands.AddProductsToShelf;

public record AddProductsToShelf(string shelfId, List<string> products) : IRequest<bool>;

public class AddProductsToShelfHandler : IRequestHandler<AddProductsToShelf, bool>
{
    private readonly IApplicationDbContext _context;
    public AddProductsToShelfHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(AddProductsToShelf request, CancellationToken cancellationToken)
    {
        var shelf = await _context.Shelves.FindAsync(request.shelfId);
        if (shelf == null)
        {
            throw new NotFoundException(nameof(Shelf), request.shelfId);
        }
        var alredyExist = await _context.ShelfProducts.Where(x => x.ShelfID == request.shelfId).ToListAsync();

        Dictionary<string, int> idCounts = new Dictionary<string, int>();
        foreach (var item in request.products)
        {
            if (idCounts.ContainsKey(item))
            {
                idCounts[item]++;
            }
            else
            {
                idCounts[item] = 1;
            }
        }
        var products = await _context.Products.Where(x => request.products.Contains(x.Id)).ToListAsync();
        if (products.Count != request.products.Count)
        {
            throw new NotFoundException(nameof(Product), "mehsullar tapilmadi");
        }
        var shelfProducts = new List<ShelfProduct>();

        foreach (var product in idCounts)
        {
            var existingProduct = alredyExist.FirstOrDefault(x => x.ProductID == product.Key);
            if (existingProduct != null)
            {
                existingProduct.Quantity += product.Value;
            }
            else
            {
                shelfProducts.Add(new ShelfProduct
                {
                    Id = Guid.NewGuid().ToString(),
                    Shelf = shelf,
                    Product = products.FirstOrDefault(x => x.Id == product.Key),
                    Quantity = product.Value
                });
            }
        }
        await _context.ShelfProducts.AddRangeAsync(shelfProducts);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
