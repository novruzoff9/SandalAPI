using Subscription.Domain.Common;
namespace Subscription.Domain.Entities;

public class SubscriptionPackage : BaseEntity
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public double Price { get; private set; }
    public int DurationInDays { get; private set; }

    private SubscriptionPackage() { }
    public SubscriptionPackage(string id, string code, string name, double price, int durationInDays)
    {
        Id = id;
        Code = code;
        Name = name;
        Price = price;
        DurationInDays = durationInDays;
    }

    public void Update(string code, string name, double price, int durationInDays)
    {
        Code = code;
        Name = name;
        Price = price;
        DurationInDays = durationInDays;
    }
}
