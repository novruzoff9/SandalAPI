using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events.DTOs.ShelfProduct
{
    public class ShelfProductDTO
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string ShelfId { get; set; }
        public string ShelfCode { get; set; }
        public string? ImageUrl { get; set; }
    }
}
