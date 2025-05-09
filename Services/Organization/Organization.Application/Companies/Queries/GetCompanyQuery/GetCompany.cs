
using AutoMapper;
using Organization.Application.Common.Interfaces;
using Organization.Application.DTOs.Company;
using Organization.Domain.Entities;

namespace Organization.Application.Companies.Queries.GetCompanyQuery;

public record GetCompany(string Id) : IRequest<CompanyDto>;

public class GetCompanyQueryHandler : IRequestHandler<GetCompany, CompanyDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCompanyQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CompanyDto> Handle(GetCompany request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        CompanyDto companyDto = _mapper.Map<CompanyDto>(company);
        return companyDto;
    }
}

