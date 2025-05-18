using Shared.Services;
using Subscription.Application.DTOs.CompanySubscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subscription.Application.Features.CompanyiesSubscriptions.Queries;

public record GetMyCompanySubscriptionQuery() : IRequest<CompanySubscriptionDto>;

public class GetMyCompanySubscriptionQueryHandler : IRequestHandler<GetMyCompanySubscriptionQuery, CompanySubscriptionDto>
{
    private readonly ISharedIdentityService _sharedIdentity; 
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetMyCompanySubscriptionQueryHandler(ISharedIdentityService sharedIdentity, IApplicationDbContext context, IMapper mapper)
    {
        _sharedIdentity = sharedIdentity;
        _context = context;
        _mapper = mapper;
    }

    public async Task<CompanySubscriptionDto> Handle(GetMyCompanySubscriptionQuery request, CancellationToken cancellationToken)
    {
        var companyId = _sharedIdentity.GetCompanyId;
        var subscription = await _context.CompanySubscriptions
           .Where(x => x.CompanyId == companyId && x.EndDate >= DateTime.UtcNow)
           .OrderByDescending(x => x.EndDate)
           .FirstOrDefaultAsync(cancellationToken);

        if (subscription == null)
        {
            throw new NotFoundException(nameof(CompanySubscription), companyId);
        }

        var package = await _context.SubscriptionPackages
            .FirstOrDefaultAsync(x => x.Id == subscription.SubscriptionPackageId, cancellationToken);

        var companySubscriptionDto = new CompanySubscriptionDto
        {
            PackageName = package.Name,
            ExpirationDate = subscription.EndDate,
            IsActive = subscription.IsActive
        };
        return companySubscriptionDto;
    }
}