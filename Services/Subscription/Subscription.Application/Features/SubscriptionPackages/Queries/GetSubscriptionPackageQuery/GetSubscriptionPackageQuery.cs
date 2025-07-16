
namespace Subscription.Application.SubscriptionPackages;

public record GetSubscriptionPackageQuery(string Id) : IRequest<SubscriptionPackage>;

public class GetSubscriptionPackageQueryHandler : IRequestHandler<GetSubscriptionPackageQuery, SubscriptionPackage>
{
    private readonly IApplicationDbContext _context;

    public GetSubscriptionPackageQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SubscriptionPackage> Handle(GetSubscriptionPackageQuery request, CancellationToken cancellationToken)
    {
        var subscriptionpackage = await _context.SubscriptionPackages.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        Guard.Against.Null(subscriptionpackage, nameof(subscriptionpackage));
        return subscriptionpackage;
    }
}

