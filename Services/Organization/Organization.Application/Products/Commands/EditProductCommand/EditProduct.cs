
namespace Organization.Application.Products.Commands.EditProductCommand;

public record EditProduct(string Id, string Name, string Description, decimal PurchasePrice, decimal SellPrice, int Quantity, string? ImageUrl
    ) : IRequest<bool>;

public class EditProductCommandHandler : IRequestHandler<EditProduct, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;

    public EditProductCommandHandler(IApplicationDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<bool> Handle(EditProduct request, CancellationToken cancellationToken)
    {
        Guard.Against.NotFound(request.Id, nameof(request));

        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (product == null) { return false; }
        if (_sharedIdentityService.GetCompanyId != product.CompanyId) { return false; }


        product.Name = request.Name;
        product.Description = request.Description;
        product.CompanyId = _sharedIdentityService.GetCompanyId;
        product.PurchasePrice = request.PurchasePrice;
        product.SellPrice = request.SellPrice;
        product.Quantity = request.Quantity;
        product.ImageUrl = request.ImageUrl;


        _context.Products.Update(product);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

