
using Organization.Application.Common.Interfaces;
using Organization.Domain.Entities;
using Shared.ResultTypes;

namespace Organization.Application.Companies.Queries.GetCompaniesQuery;

public record GetCompanies : IRequest<List<Company>>;

public class GetCompaniesQueryHandler : IRequestHandler<GetCompanies, List<Company>>
{
    private readonly IApplicationDbContext _context;

    public GetCompaniesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Company>> Handle(GetCompanies request, CancellationToken cancellationToken)
    {
        var companies = await _context.Companies.Include(x=>x.Warehouses).ToListAsync(cancellationToken);
        return companies;
    }
}

