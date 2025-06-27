using Shared.Services;

namespace Subscription.Application.Features.CompanyiesSubscriptions.Queries;

public record GetMyCompanySubscriptionHistoryQuery() : IRequest<List<CompanySubscriptionHistoryDto>>;

public class GetMyCompanySubscriptionHistoryQueryHandler : IRequestHandler<GetMyCompanySubscriptionHistoryQuery, List<CompanySubscriptionHistoryDto>>
{
    private readonly ISharedIdentityService _sharedIdentityService;
    private readonly IApplicationDbContext _context;

    public GetMyCompanySubscriptionHistoryQueryHandler(ISharedIdentityService sharedIdentityService, IApplicationDbContext applicationDbContext)
    {
        _sharedIdentityService = sharedIdentityService;
        _context = applicationDbContext;
    }

    public async Task<List<CompanySubscriptionHistoryDto>> Handle(GetMyCompanySubscriptionHistoryQuery request, CancellationToken cancellationToken)
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        if (string.IsNullOrEmpty(companyId))
        {
            throw new ArgumentException("Company ID cannot be null or empty.");
        }
        var subscriptions = _context.CompanySubscriptions
            .Where(x => x.CompanyId == companyId)
            .OrderByDescending(x => x.EndDate)
            .ToList();
        if (subscriptions == null || !subscriptions.Any())
        {
            throw new Shared.Exceptions.NotFoundException("No subscription history found for the company.");
        }

        var subscriptionPackages = await _context.SubscriptionPackages
            .Where(x => subscriptions.Select(s => s.SubscriptionPackageId).Contains(x.Id))
            .ToListAsync(cancellationToken);

        var subscriptionHistory = subscriptions.Select(subscription => new CompanySubscriptionHistoryDto
        {
            PackageName = subscriptionPackages.FirstOrDefault(x => x.Id == subscription.SubscriptionPackageId)?.Name ?? "Unknown Package",
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            IsActive = subscription.IsActive
        }).ToList();

        return subscriptionHistory;
    }
}
