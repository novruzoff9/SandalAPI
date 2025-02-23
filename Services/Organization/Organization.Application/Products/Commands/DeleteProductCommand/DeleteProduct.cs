
namespace Organization.Application.Products.Commands.DeleteProductCommand;

public record DeleteProduct(string Id) : IRequest<bool>;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProduct, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;

    public DeleteProductCommandHandler(IApplicationDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<bool> Handle(DeleteProduct request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request.Id, nameof(request.Id));

        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (product == null) { return false; }
        if (_sharedIdentityService.GetCompanyId != product.CompanyId) { return false; }
        _context.Products.Remove(product);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

