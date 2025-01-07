using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Application.Common.Models.Shelf;

public class ShelfDTO
{
    public string Id { get; set; }
    public string Code { get; set; }
    public string WarehouseID { get; set; }
    public int ItemsCount { get; set; }
}

