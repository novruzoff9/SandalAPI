using Subscription.Domain.Common;
namespace Subscription.Domain.Entities;

public class SubscriptionPackage : BaseEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int DurationInDays { get; set; }

    private SubscriptionPackage() { }
    public SubscriptionPackage(string id, string code, string name, double price, int durationInDays)
    {
        Id = id;
        Code = code;
        Name = name;
        Price = price;
        DurationInDays = durationInDays;
    }
}
