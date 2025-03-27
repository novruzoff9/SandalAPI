using Ardalis.GuardClauses;
using Order.Domain.Common;

namespace Order.Domain.Entities;

public class OrderItem : BaseEntity
{
    public string OrderId { get; set; } = null!;
    public Order? Order { get; set; }
    public string ProductId { get; private set; }
    public string ProductName { get; private set; }
    public decimal UnitPrice { get; private set; }
    public string ImageUrl { get; private set; }
    public int Quantity { get; private set; } = 1;

    public OrderItem(string productId, string productName, decimal unitPrice, string imageUrl)
    {
        Guard.Against.NullOrEmpty(productId, nameof(productId));
        Guard.Against.NullOrEmpty(productName, nameof(productName));
        Guard.Against.NullOrEmpty(imageUrl, nameof(imageUrl));
        Guard.Against.NegativeOrZero(unitPrice, nameof(unitPrice));

        Id = Guid.NewGuid().ToString();
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        ImageUrl = imageUrl;
    }

    public OrderItem(string productId, string productName, decimal unitPrice, string imageUrl, int quantity)
    {
        Guard.Against.NullOrEmpty(productId, nameof(productId));
        Guard.Against.NullOrEmpty(productName, nameof(productName));
        Guard.Against.NullOrEmpty(imageUrl, nameof(imageUrl));
        Guard.Against.NegativeOrZero(unitPrice, nameof(unitPrice));
        Guard.Against.NegativeOrZero(quantity, nameof(quantity));

        Id = Guid.NewGuid().ToString();
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        ImageUrl = imageUrl;
        Quantity = quantity;
    }

    public void Update(string productName, decimal unitPrice, string imageUrl)
    {
        this.ProductName = productName;
        this.UnitPrice = unitPrice;
        this.ImageUrl = imageUrl;
    }

    public void IncreaseQuantity(int quantity)
    {
        Guard.Against.NegativeOrZero(quantity, nameof(quantity));
        Quantity += quantity;
    }
}
