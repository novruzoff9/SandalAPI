using AutoMapper;
using Organization.Application.DTOs.Company;

namespace Organization.Application.Companies.Queries.GetCompaniesQuery;

public record GetCompanies : IRequest<List<CompanyDto>>;

public class GetCompaniesQueryHandler : IRequestHandler<GetCompanies, List<CompanyDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISharedSubscriptionService _sharedSubscriptionService;
    private readonly IMapper _mapper;
    public GetCompaniesQueryHandler(IApplicationDbContext context, IMapper mapper, ISharedSubscriptionService sharedSubscriptionService)
    {
        _context = context;
        _mapper = mapper;
        _sharedSubscriptionService = sharedSubscriptionService;
    }

    public async Task<List<CompanyDto>> Handle(GetCompanies request, CancellationToken cancellationToken)
    {
        var companies = await _context.Companies.Include(x=>x.Warehouses).ToListAsync(cancellationToken);
        List<CompanyDto> companiesDto = new();
        foreach (var company in companies)
        {
            CompanyDto companyDto = _mapper.Map<CompanyDto>(company);
            string subscription = await _sharedSubscriptionService.GetSubscriptionOfCompany(company.Id);
            companyDto.Subscription = subscription;
            companiesDto.Add(companyDto);
        }
        return companiesDto;
    }
}

