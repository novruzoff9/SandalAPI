using AutoMapper;
using Organization.Application.DTOs.Shelf;

namespace Organization.Application.Common.Mapping;

public class ShelfProductMapper : Profile
{
    public ShelfProductMapper()
    {
        CreateMap<ShelfProduct, ShelfProductDTO>()
            .ForMember(
                dest => dest.ProductName,
                opt => opt.MapFrom(src => src.Product.Name)
            )
            .ForMember(
                dest => dest.ShelfCode,
                opt => opt.MapFrom(src => src.Shelf.Code)
            )
            .ForMember(
                dest => dest.ImageUrl,
                opt => opt.MapFrom(src => src.Product.ImageUrl)
            );
    }
}
