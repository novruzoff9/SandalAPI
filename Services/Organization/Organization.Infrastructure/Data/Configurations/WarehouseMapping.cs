using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Domain.Entities;
using Shared.Services;

namespace Organization.Infrastructure.Data.Configurations;

public class WarehouseMapping : BaseEntityMapping<Warehouse>
{
    private readonly ISharedIdentityService _sharedIdentityService;
    private readonly string _currentCompanyId;

    public WarehouseMapping(ISharedIdentityService sharedIdentityService)
    {
        _sharedIdentityService = sharedIdentityService;
        _currentCompanyId = sharedIdentityService.GetCompanyId ?? throw new ArgumentNullException(nameof(sharedIdentityService), "Company ID cannot be null");
    }
    public override void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        base.Configure(builder);
        builder.HasIndex(e => e.CompanyID);
        builder.HasQueryFilter(e => e.CompanyID == _currentCompanyId);
        builder.Property(e => e.Name).HasMaxLength(256).IsRequired();


        builder.HasOne(e => e.Company)
            .WithMany(e => e.Warehouses)
            .HasForeignKey(e => e.CompanyID);
    }
}


