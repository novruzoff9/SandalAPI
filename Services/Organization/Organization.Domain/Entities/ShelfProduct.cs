using Organization.Domain.Common;

namespace Organization.Domain.Entities;

public class ShelfProduct : BaseEntity
{
    public string ShelfID { get; set; }
    public Shelf Shelf { get; set; }
    public int Quantity { get; set; }
    public string ProductID { get; set; }
    public Product Product { get; set; }
}