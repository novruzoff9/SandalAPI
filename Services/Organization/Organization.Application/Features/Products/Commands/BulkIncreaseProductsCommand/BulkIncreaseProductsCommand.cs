using Organization.Application.DTOs.Product;
using Shared.ResultTypes;

namespace Organization.Application.Features.Products.Commands.BulkIncreaseProductsCommand;

public record BulkIncreaseProductsCommand(List<ProductAndQuantityDto> IncreaseProductDtos) : IRequest<Response<bool>>;
public class BulkIncreaseProductsCommandHandler : IRequestHandler<BulkIncreaseProductsCommand, Response<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;

    public BulkIncreaseProductsCommandHandler(IApplicationDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<Response<bool>> Handle(BulkIncreaseProductsCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request.IncreaseProductDtos, nameof(request.IncreaseProductDtos), "IncreaseProductDtos cannot be null");

        var companyId = _sharedIdentityService.GetCompanyId;

        var warehouseId = await _context.Warehouses
            .AsNoTracking()
            .Where(x => x.CompanyID == companyId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (warehouseId is null)
            return Response<bool>.Fail("Warehouse not found for this company", 404);

        var shelfId = await _context.Shelves
            .AsNoTracking()
            .Where(x => x.WarehouseID == warehouseId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (shelfId is null)
            return Response<bool>.Fail("Shelf not found for this warehouse", 404);

        foreach (var dto in request.IncreaseProductDtos)
        {
            Guard.Against.NegativeOrZero(dto.Quantity, nameof(dto.Quantity), "Quantity must be greater than zero");

            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == dto.ProductId, cancellationToken);
            Guard.Against.Null(product, nameof(product), $"Product with ID {dto.ProductId} not found");

            var shelfProduct = await _context.ShelfProducts.FirstOrDefaultAsync(x => x.ProductID == dto.ProductId, cancellationToken);

            if (shelfProduct is null)
            {
                shelfProduct = new ShelfProduct
                {
                    Id = Guid.NewGuid().ToString(),
                    Quantity = dto.Quantity,
                    ShelfID = shelfId,
                    ProductID = dto.ProductId
                };
                _context.ShelfProducts.Add(shelfProduct);
            }
            else
            {
                shelfProduct.Quantity += dto.Quantity;
            }

            product.Quantity += dto.Quantity;
        }

        return await SaveChangesOrFailAsync(cancellationToken);
    }

    private async Task<Response<bool>> SaveChangesOrFailAsync(CancellationToken cancellationToken)
    {
        var saved = await _context.SaveChangesAsync(cancellationToken) > 0;
        return saved
            ? Response<bool>.Success(true, 200)
            : Response<bool>.Fail("Failed to increase product quantities", 500);
    }
}
