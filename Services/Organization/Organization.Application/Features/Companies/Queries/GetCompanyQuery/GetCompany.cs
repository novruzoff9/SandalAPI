
using AutoMapper;
using Organization.Application.Common.Interfaces;
using Organization.Application.DTOs.Company;
using Organization.Domain.Entities;

namespace Organization.Application.Features.Companies.Queries.GetCompanyQuery;

public record GetCompany(string Id) : IRequest<CompanyDto>;

public class GetCompanyQueryHandler : IRequestHandler<GetCompany, CompanyDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ISharedSubscriptionService _sharedSubscriptionService;
    private readonly IMapper _mapper;

    public GetCompanyQueryHandler(IApplicationDbContext context, IMapper mapper, ISharedSubscriptionService sharedSubscriptionService)
    {
        _context = context;
        _mapper = mapper;
        _sharedSubscriptionService = sharedSubscriptionService;
    }

    public async Task<CompanyDto> Handle(GetCompany request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies
            .Include(x => x.Warehouses)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        CompanyDto companyDto = _mapper.Map<CompanyDto>(company);
        string subscription = await _sharedSubscriptionService.GetSubscriptionOfCompanyAsync(company.Id);
        companyDto.Subscription = subscription;
        return companyDto;
    }
}

