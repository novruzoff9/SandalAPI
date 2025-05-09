using Order.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Queries;

public record GetOrdersByCustomerQuery(string id) : IRequest<List<Domain.Entities.Order>>;

public class GetOrdersByCustomerQueryHandler : IRequestHandler<GetOrdersByCustomerQuery, List<Domain.Entities.Order>>
{
    private readonly IOrderDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;
    private readonly IMapper _mapper;
    public GetOrdersByCustomerQueryHandler(IOrderDbContext context, ISharedIdentityService sharedIdentityService, IMapper mapper)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
        _mapper = mapper;
    }
    public async Task<List<Domain.Entities.Order>> Handle(GetOrdersByCustomerQuery request, CancellationToken cancellationToken)
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        var orders = await _context.Orders
            .Include(x => x.Products)
            .Include(x => x.Status)
            .Where(x => x.CompanyId == companyId && x.CustomerId == request.id)
            .ToListAsync(cancellationToken);

        return orders;
    }
}
