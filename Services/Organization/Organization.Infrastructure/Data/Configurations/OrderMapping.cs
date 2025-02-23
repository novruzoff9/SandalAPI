using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Domain.Entities;

namespace Organization.Infrastructure.Data.Configurations;

public class OrderMapping : BaseEntityMapping<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        base.Configure(builder);

        builder.HasOne(e => e.Company)
            .WithMany(e => e.Orders)
            .HasForeignKey(e => e.CompanyId);

        builder.HasOne(e => e.Warehouse)
            .WithMany(e => e.Orders)
            .HasForeignKey(e => e.WarehouseId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(e => e.Products)
            .WithOne(e => e.Order)
            .HasForeignKey(e => e.OrderId);
    }
}

