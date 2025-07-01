namespace Organization.Application.Common.Mapping;

public class ShelfProductDtoMapper : Profile
{
    public ShelfProductDtoMapper()
    {
        CreateMap<DTOs.Shelf.ShelfProductDTO, Shared.DTOs.ShelfProduct.ShelfProductDTO>()
            .ReverseMap();
    }
}
