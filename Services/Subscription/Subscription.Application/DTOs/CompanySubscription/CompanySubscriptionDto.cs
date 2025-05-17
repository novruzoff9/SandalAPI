namespace Subscription.Application.DTOs.CompanySubscription;

public class CompanySubscriptionDto
{
    public string PackageName { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsActive { get; set; }
}
