using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Domain.Entities;

namespace Organization.Infrastructure.Data.Configurations;

public class ProductMapping : BaseEntityMapping<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);
        builder.Property(e => e.Name).HasMaxLength(256).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(256);

        builder.HasOne(e => e.Company)
            .WithMany(e => e.Products)
            .HasForeignKey(e => e.CompanyId);
    }
}

