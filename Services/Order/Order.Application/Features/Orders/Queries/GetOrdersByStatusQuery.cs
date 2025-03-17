using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Queries;

public record GetOrdersByStatusQuery(string status) : IRequest<List<Domain.Entities.Order>>;

public class GetOrdersByStatusQueryHandler : IRequestHandler<GetOrdersByStatusQuery, List<Domain.Entities.Order>>
{
    private readonly IOrderDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;
    public GetOrdersByStatusQueryHandler(IOrderDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }
    public async Task<List<Domain.Entities.Order>> Handle(GetOrdersByStatusQuery request, CancellationToken cancellationToken)
    {
        var status = OrderStatus.FromName(request.status);
        string companyId = _sharedIdentityService.GetCompanyId;
        var orders = await _context.Orders
            .Where(x => x.CompanyId == companyId && x.Status == status)
            .ToListAsync(cancellationToken);

        return orders;
    }
}
