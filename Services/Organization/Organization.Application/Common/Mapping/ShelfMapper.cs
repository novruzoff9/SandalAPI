using Organization.Application.Common.Models.Shelf;

namespace Organization.Application.Common.Mapping;

public class ShelfMapper : Profile
{
    public ShelfMapper()
    {
        CreateMap<Shelf, ShelfDTO>()
            .ForMember(dest => dest.ItemsCount, 
            opt => opt.MapFrom(src => src.ShelfProducts.Sum(y => y.Quantity)));
    }
}
