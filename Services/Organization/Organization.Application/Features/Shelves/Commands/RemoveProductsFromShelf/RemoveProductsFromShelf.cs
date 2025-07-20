using Organization.Application.Common.Models.ShelfProducts;

namespace Organization.Application.Features.Shelves.Commands.RemoveProductsFromShelf;

public record RemoveProductsFromShelf(string shelfCode, List<ProductWithQuantity> products) : IRequest<bool>;

public class RemoveProductsFromShelfHandler : IRequestHandler<RemoveProductsFromShelf, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;
    public RemoveProductsFromShelfHandler(IApplicationDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }
    public async Task<bool> Handle(RemoveProductsFromShelf request, CancellationToken cancellationToken)
    {
        var warehouse = _sharedIdentityService.GetWarehouseId;
        var shelfId = await _context.Shelves.AsNoTracking()
            .Where(x => x.Code == request.shelfCode && x.WarehouseID == warehouse)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (shelfId is null)
            throw new NotFoundException(nameof(Shelf), request.shelfCode);

        var shelfProducts = await _context.ShelfProducts.Where(x => x.ShelfID == shelfId).ToListAsync();
        var products = await _context.Products.Where(x => request.products.Select(x => x.ProductId).Contains(x.Id)).ToListAsync();

        if (products.Count != request.products.Count)
            throw new NotFoundException(nameof(Product), "mehsullar tapilmadi");

        foreach (var product in request.products)
        {
            var existingProduct = shelfProducts.FirstOrDefault(x => x.ProductID == product.ProductId);
            if (existingProduct != null)
            {
                existingProduct.Quantity -= product.Quantity;
                if (existingProduct.Quantity <= 0)
                {
                    _context.ShelfProducts.Remove(existingProduct);
                }
            }
            else
                throw new NotFoundException(nameof(ShelfProduct), "mehsul tapilmadi");

            var productEntity = products.FirstOrDefault(x => x.Id == product.ProductId);
            Guard.Against.Null(productEntity, nameof(productEntity), $"Product with ID {product.ProductId} not found");
            productEntity.Quantity -= product.Quantity;
        }
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}