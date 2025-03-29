namespace Organization.Application.Companies.Commands.CreateCompanyCommand;

public record CreateCompany(string Name, string Description, string LogoUrl) : IRequest<bool>;

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompany, bool>
{
    private readonly IApplicationDbContext _context;

    public CreateCompanyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CreateCompany request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(CreateCompany));

        var company = new Company(request.Name, request.LogoUrl, request.Description);

        await _context.Companies.AddAsync(company, cancellationToken);

        var success = await _context.SaveChangesAsync(cancellationToken) > 0;

        return success;
    }
}

