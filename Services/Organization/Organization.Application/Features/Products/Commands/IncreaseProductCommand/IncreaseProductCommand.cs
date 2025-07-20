using Shared.ResultTypes;

namespace Organization.Application.Features.Products.Commands.IncreaseProductCommand;

public record IncreaseProductCommand(string Id, int Quantity) : IRequest<Response<bool>>;

public class IncreaseProductCommandHandler : IRequestHandler<IncreaseProductCommand, Response<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;

    public IncreaseProductCommandHandler(IApplicationDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<Response<bool>> Handle(IncreaseProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (product == null)
            return Response<bool>.Fail("Product not found", 404);

        if (request.Quantity <= 0)
            return Response<bool>.Fail("Quantity must be greater than zero", 400);

        var shelfProduct = await _context.ShelfProducts.FirstOrDefaultAsync(x => x.ProductID == request.Id, cancellationToken);

        if (shelfProduct is null)
        {
            string companyId = _sharedIdentityService.GetCompanyId;

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

            _context.ShelfProducts.Add(new ShelfProduct
            {
                Id = Guid.NewGuid().ToString(),
                Quantity = request.Quantity,
                ShelfID = shelfId,
                ProductID = request.Id
            });
        }
        else
        {
            shelfProduct.Quantity += request.Quantity;
        }

        product.Quantity += request.Quantity;

        return await SaveChangesOrFailAsync(cancellationToken);
    }

    private async Task<Response<bool>> SaveChangesOrFailAsync(CancellationToken cancellationToken)
    {
        bool saved = await _context.SaveChangesAsync(cancellationToken) > 0;
        return saved
            ? Response<bool>.Success(true, 200)
            : Response<bool>.Fail("Failed to increase product quantity", 500);
    }
}