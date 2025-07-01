using Organization.Application.DTOs.Warehouse;

namespace Organization.Application.Common.Mapping;

public class WarehouseMapper : Profile
{
    public WarehouseMapper()
    {
        CreateMap<Warehouse, WarehouseDto>()
            .ForMember(
                dest => dest.Shelves,
                opt => opt.MapFrom(src => src.Shelves.Count)
            );
    }
}