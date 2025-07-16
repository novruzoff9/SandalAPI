
namespace Subscription.Application.SubscriptionPackages;

public record DeleteSubscriptionPackageCommand(string Id) : IRequest<bool>;

public class DeleteSubscriptionPackageCommandHandler : IRequestHandler<DeleteSubscriptionPackageCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteSubscriptionPackageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteSubscriptionPackageCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request.Id, nameof(request.Id));

        var subscriptionpackage = await _context.SubscriptionPackages.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        Guard.Against.Null(subscriptionpackage, nameof(subscriptionpackage));

        _context.SubscriptionPackages.Remove(subscriptionpackage);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

