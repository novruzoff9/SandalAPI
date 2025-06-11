using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Domain.Entities;

namespace Organization.Infrastructure.Data.Configurations;

public class CompanyMapping : BaseEntityMapping<Company>
{
    public override void Configure(EntityTypeBuilder<Company> builder)
    {
        base.Configure(builder);
        builder.Property(e => e.Name).HasMaxLength(256).IsRequired();
        builder.Property(e => e.LogoUrl).HasMaxLength(256);

        builder.HasMany(e => e.Warehouses)
            .WithOne(e => e.Company)
            .HasForeignKey(e => e.CompanyID);

        builder.HasMany(e => e.Products)
            .WithOne(e => e.Company)
            .HasForeignKey(e => e.CompanyId);
    }
}


