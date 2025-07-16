
namespace Subscription.Application.SubscriptionPackages;

public record GetSubscriptionPackagesQuery : IRequest<List<SubscriptionPackage>>;

public class GetSubscriptionPackagesQueryHandler : IRequestHandler<GetSubscriptionPackagesQuery, List<SubscriptionPackage>>
{
    private readonly IApplicationDbContext _context;

    public GetSubscriptionPackagesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubscriptionPackage>> Handle(GetSubscriptionPackagesQuery request, CancellationToken cancellationToken)
    {
        var subscriptionpackages = await _context.SubscriptionPackages.OrderBy(x=>x.Price).ToListAsync(cancellationToken);
        return subscriptionpackages;
    }
}

