using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Infrastructure.Data;

public class OrderDbContextSeed
{
    public async Task SeedAsync(OrderDbContext context)
    {
        // Seed initial data for OrderDbContext if needed
        // This could include seeding default order statuses, etc.
        // Example:
        // await _context.OrderStatuses.AddRangeAsync(OrderStatus.List());
        // await _context.SaveChangesAsync();
    }
}
