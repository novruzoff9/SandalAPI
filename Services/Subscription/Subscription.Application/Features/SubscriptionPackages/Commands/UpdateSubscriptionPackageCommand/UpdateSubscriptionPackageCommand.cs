
namespace Subscription.Application.SubscriptionPackages;

public record UpdateSubscriptionPackageCommand(string Id, string Code, string Name, double Price, int DurationInDays) : IRequest<bool>;

public class UpdateSubscriptionPackageCommandHandler : IRequestHandler<UpdateSubscriptionPackageCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateSubscriptionPackageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateSubscriptionPackageCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NotFound(request.Id, nameof(request));

        var subscriptionpackage = await _context.SubscriptionPackages.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        Guard.Against.Null(subscriptionpackage, nameof(subscriptionpackage));

        subscriptionpackage.Update(
            code: request.Code,
            name: request.Name,
            price: request.Price,
            durationInDays: request.DurationInDays
        );

        _context.SubscriptionPackages.Update(subscriptionpackage);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

