using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Domain.Entities;

namespace Organization.Infrastructure.Data.Configurations;

public class CustomerMapping : BaseEntityMapping<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        base.Configure(builder);
        builder.Property(e => e.CompanyId).HasMaxLength(36).IsRequired();

        builder.HasOne(e => e.Company)
            .WithMany(e=>e.Customers)
            .HasForeignKey(e => e.CompanyId);
    }
}
