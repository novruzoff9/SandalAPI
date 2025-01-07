namespace Organization.Application.Companies.Commands.DeleteCompanyCommand;

public record DeleteCompany(string Id) : IRequest<bool>;

public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompany, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteCompanyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteCompany request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request.Id, nameof(request.Id));

        var company = await _context.Companies.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (company == null) { return false; }
        _context.Companies.Remove(company);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

