using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Domain.Entities;
using Shared.Services;

namespace Organization.Infrastructure.Data.Configurations;

public class ProductMapping : BaseEntityMapping<Product>
{
    private readonly ISharedIdentityService _sharedIdentityService;
    private readonly string _currentCompanyId;

    public ProductMapping(ISharedIdentityService sharedIdentityService)
    {
        _sharedIdentityService = sharedIdentityService;
        _currentCompanyId = sharedIdentityService.GetCompanyId ?? throw new ArgumentNullException(nameof(sharedIdentityService), "Company ID cannot be null");
    }

    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);
        builder.HasIndex(e => e.CompanyId);
        builder.HasQueryFilter(e => e.CompanyId == _currentCompanyId);
        builder.Property(e => e.Name).HasMaxLength(256).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(256);

        builder.HasOne(e => e.Company)
            .WithMany(e => e.Products)
            .HasForeignKey(e => e.CompanyId);
    }
}

