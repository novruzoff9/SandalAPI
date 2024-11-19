using Organization.Domain.Common;

namespace Organization.Domain.Entities;

public class Shelf : BaseEntity
{
    public string Code { get; set; }
    public string WarehouseID { get; set; }
    public Warehouse Warehouse { get; set; }
    public List<ShelfProduct> ShelfProducts { get; set; }
    public int ItemsCount => this.ShelfProducts.Count();
}
