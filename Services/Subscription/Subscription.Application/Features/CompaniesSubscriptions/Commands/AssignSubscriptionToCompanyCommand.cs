using EventBus.Base.Abstraction;
using Shared.Events;

namespace Subscription.Application.Features.CompaniesSubscriptions.Commands;

public record AssignSubscriptionToCompanyCommand(string CompanyId, string PackageCode) : IRequest<string>;

public class AssignSubscriptionToCompanyCommandHandler : IRequestHandler<AssignSubscriptionToCompanyCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IEventBus _eventBus;

    public AssignSubscriptionToCompanyCommandHandler(IApplicationDbContext context, IEventBus eventBus)
    {
        _context = context;
        _eventBus = eventBus;
    }

    public async Task<string> Handle(AssignSubscriptionToCompanyCommand request, CancellationToken cancellationToken)
    {
        var package = await _context.SubscriptionPackages
            .FirstOrDefaultAsync(x => x.Code == request.PackageCode, cancellationToken);
        if (package == null)
        {
            return new Common.Exceptions.NotFoundException("Abunelik")
                .ToString();
            //throw new NotFoundException(nameof(SubscriptionPackage), request.PackageCode);
        }

        var companySubscription = await _context.CompanySubscriptions
            .FirstOrDefaultAsync(x => x.CompanyId == request.CompanyId && x.EndDate > DateTime.Now);

        if(companySubscription != null)
        {
            if(companySubscription.SubscriptionPackageId == package.Id)
            {
                return companySubscription.Id;
            }
            else
            {
                companySubscription.TerminateCurrentSubscription();
                _context.CompanySubscriptions.Update(companySubscription);
            }
        }

        int durationInDays = package.DurationInDays;
        var subscription = new CompanySubscription(request.CompanyId, package.Id, DateTime.Now, DateTime.Now.AddDays(durationInDays));

        await _context.CompanySubscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync(cancellationToken);

        // Publish an integration event
        var @event = new CompanyAssignedPackIntegrationEvent(request.CompanyId, package.Id, package.Name, subscription.EndDate);
        _eventBus.Publish(@event);

        return subscription.Id;
    }
}
