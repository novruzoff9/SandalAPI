using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Entities;

namespace Order.Infrastructure.Data.Configurations;

public class OrderItemMapping : BaseEntityMapping<OrderItem>
{
    private readonly ISharedIdentityService _sharedIdentityService;

    public OrderItemMapping(ISharedIdentityService sharedIdentityService)
    {
        _sharedIdentityService = sharedIdentityService;
    }

    public override void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        base.Configure(builder);
        string companyId = _sharedIdentityService.GetCompanyId;

        builder.ToTable("OrderItems", OrderDbContext.DEFAULT_SCHEMA);
        builder.HasIndex(e => e.OrderId);

        builder.HasOne(e => e.Order)
            .WithMany(e => e.Products)
            .HasForeignKey(e => e.OrderId);
    }
}

