using AutoMapper;
using Organization.Application.DTOs.Company;

namespace Organization.Application.Common.Mapping;

public class CompanyMapper : Profile
{
    public CompanyMapper()
    {
        CreateMap<Company, CompanyDto>()
            .ForMember(
                dest => dest.Warehouses,
                opt => opt.MapFrom(src => src.Warehouses.Count())
            );
    }
}
