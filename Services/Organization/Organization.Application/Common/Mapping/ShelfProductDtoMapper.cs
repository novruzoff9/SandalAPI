using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Application.Common.Mapping;

public class ShelfProductDtoMapper : Profile
{
    public ShelfProductDtoMapper()
    {
        CreateMap<DTOs.Shelf.ShelfProductDTO, Shared.Events.DTOs.ShelfProduct.ShelfProductDTO>()
            .ReverseMap();
    }
}
