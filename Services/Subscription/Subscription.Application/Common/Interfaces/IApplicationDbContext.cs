using System.Threading;
using System.Threading.Tasks;

namespace Subscription.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<SubscriptionPackage> SubscriptionPackages { get; set; }
        public DbSet<CompanySubscription> CompanySubscriptions { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
