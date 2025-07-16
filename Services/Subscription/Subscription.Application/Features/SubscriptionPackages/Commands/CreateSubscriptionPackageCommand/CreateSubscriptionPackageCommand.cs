
namespace Subscription.Application.SubscriptionPackages;

public record CreateSubscriptionPackageCommand(string Code, string Name, double Price, int DurationInDays) : IRequest<bool>;

public class CreateSubscriptionPackageCommandHandler : IRequestHandler<CreateSubscriptionPackageCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public CreateSubscriptionPackageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CreateSubscriptionPackageCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(CreateSubscriptionPackageCommand));

        string id = Guid.NewGuid().ToString();
        var subscriptionpackage = new SubscriptionPackage(id, request.Code, request.Name, request.Price, request.DurationInDays);

        await _context.SubscriptionPackages.AddAsync(subscriptionpackage, cancellationToken);

        var success = await _context.SaveChangesAsync(cancellationToken) > 0;

        return success;
    }
}

