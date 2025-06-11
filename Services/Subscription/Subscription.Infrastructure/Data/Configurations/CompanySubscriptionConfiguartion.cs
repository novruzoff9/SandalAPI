using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Services;
using Subscription.Domain.Entities;

namespace Subscription.Infrastructure.Data.Configurations;

public class CompanySubscriptionConfiguartion : BaseEntityMapping<CompanySubscription>
{
    private readonly ISharedIdentityService _sharedIdentityService;

    public CompanySubscriptionConfiguartion(ISharedIdentityService sharedIdentityService)
    {
        _sharedIdentityService = sharedIdentityService;
    }

    public override void Configure(EntityTypeBuilder<CompanySubscription> builder)
    {
        base.Configure(builder);

        string companyId = _sharedIdentityService.GetCompanyId
            ?? throw new ArgumentNullException(nameof(_sharedIdentityService), "Company ID cannot be null");

        builder.HasIndex(e => e.CompanyId);
        builder.HasQueryFilter(e => e.CompanyId == companyId);
    }
}
