using Shared.Services;

namespace Subscription.Application.Features.CompaniesSubscriptions.Queries;

public record GetMyCompanyActiveSubscriptionQuery() : IRequest<CompanySubscriptionDto>;

public class GetMyCompanySubscriptionQueryHandler : IRequestHandler<GetMyCompanyActiveSubscriptionQuery, CompanySubscriptionDto>
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

    public async Task<CompanySubscriptionDto> Handle(GetMyCompanyActiveSubscriptionQuery request, CancellationToken cancellationToken)
    {
        var companyId = _sharedIdentity.GetCompanyId;
        //TODO: Subscription melumati Redis-den gelecek
        var subscription = await _context.CompanySubscriptions
           .Where(x => x.CompanyId == companyId && x.EndDate >= DateTime.UtcNow)
           .OrderByDescending(x => x.EndDate)
           .FirstOrDefaultAsync(cancellationToken);

        Guard.Against.Null(subscription, nameof(subscription), "No active subscription found for the company");

        var package = await _context.SubscriptionPackages
            .FirstOrDefaultAsync(x => x.Id == subscription.SubscriptionPackageId, cancellationToken);
        Guard.Against.Null(package, nameof(package), "Subscription package not found");

        var companySubscriptionDto = new CompanySubscriptionDto
        {
            PackageName = package.Name,
            ExpirationDate = subscription.EndDate,
            IsActive = subscription.IsActive
        };
        return companySubscriptionDto;
    }
}