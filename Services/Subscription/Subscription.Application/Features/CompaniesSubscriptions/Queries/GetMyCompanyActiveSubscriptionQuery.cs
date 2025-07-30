using Shared.Services;

namespace Subscription.Application.Features.CompaniesSubscriptions.Queries;

public record GetMyCompanyActiveSubscriptionQuery() : IRequest<CompanySubscriptionDto>;

public class GetMyCompanySubscriptionQueryHandler : IRequestHandler<GetMyCompanyActiveSubscriptionQuery, CompanySubscriptionDto>
{
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _sharedIdentity; 
    private readonly ISharedSubscriptionService _sharedSubscription;

    public GetMyCompanySubscriptionQueryHandler(ISharedIdentityService sharedIdentity, IApplicationDbContext context, IMapper mapper, ISharedSubscriptionService sharedSubscriptionService)
    {
        _mapper = mapper;
        _context = context;
        _sharedIdentity = sharedIdentity;
        _sharedSubscription = sharedSubscriptionService;
    }

    public async Task<CompanySubscriptionDto> Handle(GetMyCompanyActiveSubscriptionQuery request, CancellationToken cancellationToken)
    {
        var companyId = _sharedIdentity.GetCompanyId;
        var subscription = await _sharedSubscription.GetSubscriptionAsync();

        Guard.Against.Null(subscription, nameof(subscription), "No active subscription found for the company");

        var package = await _context.SubscriptionPackages
            .FirstOrDefaultAsync(x => x.Id == subscription.PackageId, cancellationToken);
        Guard.Against.Null(package, nameof(package), "Subscription package not found");

        var companySubscriptionDto = new CompanySubscriptionDto
        {
            PackageName = package.Name,
            ExpirationDate = subscription.ExpiredTime,
            IsActive = subscription.ExpiredTime >= DateTime.UtcNow,
        };
        return companySubscriptionDto;
    }
}