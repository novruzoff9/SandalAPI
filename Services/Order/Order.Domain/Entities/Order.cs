using Ardalis.GuardClauses;
using Order.Domain.Common;
using Order.Domain.ValueObjects;

namespace Order.Domain.Entities;

public class Order : BaseEntity, IAggregateRoot
{
    public DateTime Opened { get; private set; }
    public string OpenedBy { get; private set; } = null!;
    public DateTime? Closed { get; private set; }
    public string? ClosedBy { get; private set; }
    public string CompanyId { get; private set; }
    public string WarehouseId { get; private set; }
    public string WarehouseName { get; private set; }
    public string CustomerId { get; private set; } = null!;
    public string? Note { get; private set; }

    public Address Address { get; private set; }

    private readonly List<OrderItem> _products = new();
    public IReadOnlyCollection<OrderItem> Products => _products.AsReadOnly();

    private int _statusId;
    public OrderStatus? Status { get; private set; }

    public Order() { }
    public Order(string openedBy,Address address, string companyId,
        string warehouseId, string warehouseName,
        string customerId)
    {
        Guard.Against.Null(address, nameof(address));
        Guard.Against.NullOrEmpty(companyId, nameof(companyId));
        Guard.Against.NullOrEmpty(warehouseId, nameof(warehouseId));
        Guard.Against.NullOrEmpty(warehouseName, nameof(warehouseName));
        //Guard.Against.NullOrEmpty(customerId, nameof(customerId));

        Id = Guid.NewGuid().ToString();
        Opened = DateTime.Now;
        OpenedBy = openedBy;
        Address = address;
        CompanyId = companyId;
        WarehouseId = warehouseId;
        WarehouseName = warehouseName;
        CustomerId = customerId;
        _statusId = OrderStatus.Submitted.Id;
        //CustomerId = customerId;
        _products = new List<OrderItem>();
    }

    public void AddOrderItem(string productId, string productName, decimal unitPrice, string imageUrl)
    {
        Guard.Against.NullOrEmpty(productId, nameof(productId));
        Guard.Against.NullOrEmpty(productName, nameof(productName));
        Guard.Against.NullOrEmpty(imageUrl, nameof(imageUrl));
        Guard.Against.NegativeOrZero(unitPrice, nameof(unitPrice));

        var existingOrderItem = _products.FirstOrDefault(o => o.ProductId == productId);
        if (existingOrderItem == null)
        {
            _products.Add(new OrderItem(productId, productName, unitPrice, imageUrl));
        }
        else
        {
            existingOrderItem.IncreaseQuantity(1);
        }
    }

    public void AddOrderItem(string productId, string productName, decimal unitPrice, string imageUrl, int quantity)
    {
        Guard.Against.NullOrEmpty(productId, nameof(productId));
        Guard.Against.NullOrEmpty(productName, nameof(productName));
        Guard.Against.NullOrEmpty(imageUrl, nameof(imageUrl));
        Guard.Against.NegativeOrZero(unitPrice, nameof(unitPrice));
        Guard.Against.NegativeOrZero(quantity, nameof(quantity));

        var existingOrderItem = _products.FirstOrDefault(o => o.ProductId == productId);
        if (existingOrderItem == null)
        {
            _products.Add(new OrderItem(productId, productName, unitPrice, imageUrl, quantity));
        }
        else
        {
            existingOrderItem.IncreaseQuantity(quantity);
        }
    }

    public void RemoveOrderItem(string productId)
    {
        Guard.Against.NullOrEmpty(productId, nameof(productId));
        var existingOrderItem = _products.FirstOrDefault(o => o.ProductId == productId);
        if (existingOrderItem != null)
        {
            _products.Remove(existingOrderItem);
        }
    }

    public void UpdateAddress(Address address)
    {
        Guard.Against.Null(address, nameof(address));
        Address = address;
    }

    public void Close(string closedBy)
    {
        Guard.Against.NullOrEmpty(closedBy, nameof(closedBy));
        Closed = DateTime.Now;
        ClosedBy = closedBy;
    }

    public void UpdateStatus(OrderStatus status)
    {
        Guard.Against.Null(status, nameof(status));
        _statusId = status.Id;
    }

    public void UpdateNote(string note)
    {
        Note = note;
    }
}
