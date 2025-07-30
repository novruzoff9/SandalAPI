using Microsoft.EntityFrameworkCore.Storage;

namespace Organization.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Shelf> Shelves { get; set; }
    public DbSet<ShelfProduct> ShelfProducts { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<StockHistory> StockHistories { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}
