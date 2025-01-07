using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Domain.Entities;

namespace Organization.Infrastructure.Data.Configurations;

public class OrderItemMapping : BaseEntityMapping<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        base.Configure(builder);
        builder.HasOne(e => e.Order)
            .WithMany(e => e.Products)
            .HasForeignKey(e => e.OrderId);
        builder.HasOne(e => e.Product)
            .WithMany(e => e.OrderItems)
            .HasForeignKey(e => e.ProductId);
    }
}

