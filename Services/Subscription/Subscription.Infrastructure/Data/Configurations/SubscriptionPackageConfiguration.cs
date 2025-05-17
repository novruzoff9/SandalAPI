using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Subscription.Domain.Constants;
using Subscription.Domain.Entities;

namespace Subscription.Infrastructure.Data.Configurations;
public class SubscriptionPackageConfiguration : BaseEntityMapping<SubscriptionPackage>
{
    public override void Configure(EntityTypeBuilder<SubscriptionPackage> builder)
    {
        base.Configure(builder);

        builder.HasIndex(e => e.Code)
            .IsUnique();

        builder.HasData(
            new SubscriptionPackage(
                id: "93fc1c53-0c61-4f4d-b02e-82d94310a2e6",
                code: SubscriptionConstants.Bronze,
                name: "Bronze",
                price: 0,
                durationInDays: 30
            ),
            new SubscriptionPackage(
                id: "c4a18e01-2d9f-4e4f-9a30-0b5eab0e24b0",
                code: SubscriptionConstants.Silver,
                name: "Silver",
                price: 49.99d,
                durationInDays: 30
            ),
            new SubscriptionPackage(
                id: "b2f0b276-6b39-45b6-9db8-c6fe7aab889e",
                code: SubscriptionConstants.Gold,
                name: "Gold",
                price: 199.99d,
                durationInDays: 30
            )
        );
    }
}
