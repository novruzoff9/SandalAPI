using System.Threading;
using System.Threading.Tasks;

namespace Order.Application.Common.Interfaces;

public interface IOrderDbContext
{
    public DbSet<Domain.Entities.Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderStatus> OrderStatuses { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
