using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Application.Common.Models.ShelfProducts;

public class AddingProductsToShelfDTO
{
    public string ShelfCode { get; set; }
    public Dictionary<string, int> ProductIds { get; set; }
}

public class RemoveProductsFromShelfDTO
{
    public string ShelfCode { get; set; }
    public List<ProductWithQuantity> Products { get; set; }
}

public class ProductWithQuantity
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
}
