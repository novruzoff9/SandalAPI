using Order.Application.DTOs.Order;

namespace Order.Application.Common.Mapping;

public class OrderItemMapper : Profile
{
    public OrderItemMapper()
    {
        CreateMap<Order.Domain.Entities.OrderItem, OrderItemShowDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProductName));
    }
}
