﻿using System;
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
