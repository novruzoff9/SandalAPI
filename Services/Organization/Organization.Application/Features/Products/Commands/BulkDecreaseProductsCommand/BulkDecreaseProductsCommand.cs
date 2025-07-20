using Organization.Application.DTOs.Product;

namespace Organization.Application.Features.Products.Commands.BulkDecreaseProductsCommand;

public record BulkDecreaseProductsCommand(List<ProductAndQuantityDto> DecreaseProductDtos) : IRequest<bool>;

public class BulkDecreaseProductsCommandHandler : IRequestHandler<BulkDecreaseProductsCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public BulkDecreaseProductsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(BulkDecreaseProductsCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request.DecreaseProductDtos, nameof(request.DecreaseProductDtos), "DecreaseProductDtos cannot be null");

        foreach (var decreaseProductDto in request.DecreaseProductDtos)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == decreaseProductDto.ProductId);
            Guard.Against.Null(product, nameof(product), $"Product with ID {decreaseProductDto.ProductId} not found");
            Guard.Against.NegativeOrZero(decreaseProductDto.Quantity, nameof(decreaseProductDto.Quantity), "Quantity must be greater than zero");

            if (product.Quantity < decreaseProductDto.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for product with ID {decreaseProductDto.ProductId}. Available: {product.Quantity}, Requested: {decreaseProductDto.Quantity}");
            }
            var shelfProduct = _context.ShelfProducts.FirstOrDefault(x => x.ProductID == decreaseProductDto.ProductId);
            Guard.Against.Null(shelfProduct, nameof(shelfProduct), $"ShelfProduct with ProductID {decreaseProductDto.ProductId} not found");
            if (shelfProduct.Quantity < decreaseProductDto.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock in shelf for product with ID {decreaseProductDto.ProductId}. Available: {shelfProduct.Quantity}, Requested: {decreaseProductDto.Quantity}");
            }
            shelfProduct.Quantity -= decreaseProductDto.Quantity;
            product.Quantity -= decreaseProductDto.Quantity;
        }
        bool result = await _context.SaveChangesAsync(cancellationToken) > 0;
        if (!result)
        {
            throw new Exception("Failed to decrease product quantities");
        }
        return true;
    }
}