using Organization.Domain.Common;

namespace Organization.Domain.Entities;

public class Shelf : BaseEntity
{
    public string Code { get; private set; }
    public string WarehouseID { get; private set; }
    public Warehouse? Warehouse { get; }
    public IReadOnlyCollection<ShelfProduct>? ShelfProducts { get; }
    private Shelf() { }
    public Shelf(string code, string warehouseId)
    {
        Id = Guid.NewGuid().ToString();
        Code = code;
        WarehouseID = warehouseId;
    }

    public void Update(string code, string warehouseId)
    {
        Code = code;
        WarehouseID = warehouseId;
    }
}
