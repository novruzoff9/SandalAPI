namespace Order.Domain.Entities;

public class OrderStatus : Enumeration
{
    public static OrderStatus Submitted = new(1, nameof(Submitted).ToLowerInvariant());
    public static OrderStatus StockConfirmed = new(2, nameof(StockConfirmed).ToLowerInvariant());
    public static OrderStatus InProgress = new(6, nameof(InProgress).ToLowerInvariant());
    public static OrderStatus Prepared = new(3, nameof(Prepared).ToLowerInvariant());
    public static OrderStatus Shipped = new(4, nameof(Shipped).ToLowerInvariant());
    public static OrderStatus Cancelled = new(5, nameof(Cancelled).ToLowerInvariant());


    public OrderStatus(int id, string name) : base(id, name)
    {
        
    }


    public static IEnumerable<OrderStatus> List() =>
        new[] { Submitted, StockConfirmed, InProgress, Prepared, Shipped, Cancelled };

    public static OrderStatus FromName(string name)
    {
        var state = List()
            .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

        return state ?? throw new OrderingDomainException($"Possible values for OrderStatus: {String.Join(",", List().Select(s => s.Name))}");
    }

    public static OrderStatus From(int id)
    {
        var state = List().SingleOrDefault(s => s.Id == id);

        return state ?? throw new OrderingDomainException($"Possible values for OrderStatus: {String.Join(",", List().Select(s => s.Name))}");
    }
}
