using Order.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Common.Mapping;

public class OrderMapper : Profile
{
    public OrderMapper()
    {
        CreateMap<Order.Domain.Entities.Order, OrderShowDto>()
            .ForMember(dest => dest.Warehouse, opt => opt.MapFrom(src => src.WarehouseName))
            .ForMember(dest => dest.Opened, opt => opt.MapFrom(src => src.Opened.ToString("yyyy-MM-dd HH:mm")))
            .ForMember(dest => dest.OpenedBy, opt => opt.MapFrom(src => src.OpenedBy))
            .ForMember(dest => dest.Closed, opt => opt.MapFrom(src => src.Closed.HasValue ? src.Closed.Value.ToString("yyyy-MM-dd HH:mm") : null))
            .ForMember(dest => dest.ClosedBy, opt => opt.MapFrom(src => src.ClosedBy != null ? src.ClosedBy : null))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Closed.HasValue ? "Closed" : "Open"));
    }
}
