using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Domain.Entities;
using Shared.Services;

namespace Organization.Infrastructure.Data.Configurations;

public class CustomerMapping : BaseEntityMapping<Customer>
{
    private readonly ISharedIdentityService _sharedIdentityService;

    public CustomerMapping(ISharedIdentityService sharedIdentityService)
    {
        _sharedIdentityService = sharedIdentityService;
    }

    public override void Configure(EntityTypeBuilder<Customer> builder)
    {
        base.Configure(builder);
        string companyId = _sharedIdentityService.GetCompanyId
            ?? throw new ArgumentNullException(nameof(_sharedIdentityService), "Company ID cannot be null");

        builder.OwnsOne(e => e.Address);

        builder.HasIndex(e => e.CompanyId);

        builder.HasQueryFilter(e => e.CompanyId == companyId);

        builder.HasOne(e => e.Company)
            .WithMany(e=>e.Customers)
            .HasForeignKey(e => e.CompanyId);
    }
}
