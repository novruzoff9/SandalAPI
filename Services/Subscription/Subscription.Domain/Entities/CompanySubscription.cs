using Subscription.Domain.Common;

namespace Subscription.Domain.Entities;

public class CompanySubscription : BaseEntity
{
    public string CompanyId { get; private set; }
    public string SubscriptionPackageId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsActive => EndDate > DateTime.UtcNow;
    private CompanySubscription() { }

    public CompanySubscription(string companyId, string subscriptionPackageId, DateTime startDate, DateTime endDate)
    {
        Id = Guid.NewGuid().ToString();
        CompanyId = companyId;
        SubscriptionPackageId = subscriptionPackageId;
        StartDate = startDate;
        EndDate = endDate;
    }

    public void TerminateCurrentSubscription()
    {
        EndDate = DateTime.Now;
    }
}
