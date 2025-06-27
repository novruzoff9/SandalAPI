namespace Subscription.Application.Features.CompanyiesSubscriptions.Queries;

public record GetCompanyActiveSubscriptionQuery(string CompanyId) : IRequest<CompanySubscriptionDto>;

public class GetCompanyActiveSubscriptionQueryHandler : IRequestHandler<GetCompanyActiveSubscriptionQuery, CompanySubscriptionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetCompanyActiveSubscriptionQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<CompanySubscriptionDto> Handle(GetCompanyActiveSubscriptionQuery request, CancellationToken cancellationToken)
    {
        var subscription = await _context.CompanySubscriptions
            .Where(x => x.CompanyId == request.CompanyId && x.EndDate >= DateTime.UtcNow)
            .OrderByDescending(x => x.EndDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (subscription == null)
        {
            throw new NotFoundException(nameof(CompanySubscription), request.CompanyId);
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

