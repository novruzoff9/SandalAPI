using Organization.Domain.Common;

namespace Organization.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string CompanyID { get; set; }
    public Company Company { get; set; }
    public List<ShelfProduct> ShelfProducts { get; set; }
    public List<OrderItem> OrderItems { get; set; }
}
