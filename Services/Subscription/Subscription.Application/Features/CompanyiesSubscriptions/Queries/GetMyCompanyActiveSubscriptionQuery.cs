using Shared.Services;

namespace Subscription.Application.Features.CompanyiesSubscriptions.Queries;

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