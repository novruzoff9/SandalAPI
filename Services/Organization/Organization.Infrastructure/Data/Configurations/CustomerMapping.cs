using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Domain.Entities;
using Shared.Services;

namespace Organization.Infrastructure.Data.Configurations;

public class CustomerMapping : BaseEntityMapping<Customer>
{
    public override void Configure(EntityTypeBuilder<Customer> builder)
    {
        base.Configure(builder);

        builder.OwnsOne(e => e.Address);

        builder.HasIndex(e => e.CompanyId);

        builder.HasOne(e => e.Company)
            .WithMany(e=>e.Customers)
            .HasForeignKey(e => e.CompanyId);
    }
}
