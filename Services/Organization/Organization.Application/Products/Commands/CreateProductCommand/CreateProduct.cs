using Organization.Application.Common.Services;

namespace Organization.Application.Products.Commands.CreateProductCommand;

public record CreateProduct(string Name, string Description) : IRequest<bool>;

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
            CompanyID = _sharedIdentityService.GetCompanyId,
        };

        await _context.Products.AddAsync(product, cancellationToken);

        var success = await _context.SaveChangesAsync(cancellationToken) > 0;

        return success;
    }
}

