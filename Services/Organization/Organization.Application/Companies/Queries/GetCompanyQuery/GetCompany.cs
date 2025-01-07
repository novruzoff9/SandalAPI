
using Organization.Application.Common.Interfaces;
using Organization.Domain.Entities;

namespace Organization.Application.Companies.Queries.GetCompanyQuery;

public record GetCompany(string Id) : IRequest<Company>;

public class GetCompanyQueryHandler : IRequestHandler<GetCompany, Company>
{
    private readonly IApplicationDbContext _context;

    public GetCompanyQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Company> Handle(GetCompany request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return company;
    }
}

