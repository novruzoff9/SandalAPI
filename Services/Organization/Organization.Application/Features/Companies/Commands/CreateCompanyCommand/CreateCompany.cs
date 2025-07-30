namespace Organization.Application.Features.Companies.Commands.CreateCompanyCommand;

public record CreateCompany(string Name, string Description, string LogoUrl) : IRequest<bool>;

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompany, bool>
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _sharedIdentity;

    public CreateCompanyCommandHandler(IApplicationDbContext context, IMediator mediator, ISharedIdentityService sharedIdentity)
    {
        _context = context;
        _mediator = mediator;
        _sharedIdentity = sharedIdentity;
    }

    public async Task<bool> Handle(CreateCompany request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(CreateCompany));
        string role = _sharedIdentity.GetRole;
        if (role != "admin")
            throw new UnauthorizedAccessException("Sizin ?irk?t yaratmaq üçün icaz?niz yoxdur");

        using var transaction = await _context.BeginTransactionAsync(cancellationToken);

        try
        {
            var company = new Company(request.Name, request.LogoUrl, request.Description);
            await _context.Companies.AddAsync(company, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var warehouse = new Warehouse(
                "Main Warehouse",
                "Default City",
                "Default State",
                "Default Street",
                "00000",
                company.Id
            );
            await _context.Warehouses.AddAsync(warehouse, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var shelf = new Shelf("1A", warehouse.Id);
            await _context.Shelves.AddAsync(shelf, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new Exception("Failed to create company with defaults", ex);
        }
    }
}

