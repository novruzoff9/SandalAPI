using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Services;
using Subscription.Domain.Entities;

namespace Subscription.Infrastructure.Data.Configurations;

public class CompanySubscriptionConfiguartion : BaseEntityMapping<CompanySubscription>
{
    public override void Configure(EntityTypeBuilder<CompanySubscription> builder)
    {
        base.Configure(builder);

        builder.HasIndex(e => e.CompanyId);
    }
}
