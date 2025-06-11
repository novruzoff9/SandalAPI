using Organization.Application.Common.Services;

namespace Organization.Application.Products.Commands.CreateProductCommand;

public record CreateProduct(
    string Name, string Description, decimal PurchasePrice, decimal SellPrice, int Quantity, int MinRequire, string? ImageUrl
    ) : IRequest<bool>;

public class CreateProductCommandHandler : IRequestHandler<CreateProduct, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;

    public CreateProductCommandHandler(IApplicationDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<bool> Handle(CreateProduct request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(CreateProduct));

        var product = new Product
        {
            Id = RandomIdService.GenerateRandomCode(16),
            Name = request.Name,
            Description = request.Description,
            CompanyId = _sharedIdentityService.GetCompanyId,
            PurchasePrice = request.PurchasePrice,
            SellPrice = request.SellPrice,
            Quantity = request.Quantity,
            MinRequire = request.MinRequire,
            ImageUrl = request.ImageUrl
        };

        await _context.Products.AddAsync(product, cancellationToken);

        var success = await _context.SaveChangesAsync(cancellationToken) > 0;

        return success;
    }
}

