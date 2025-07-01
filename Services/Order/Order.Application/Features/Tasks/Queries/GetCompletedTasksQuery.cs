using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Tasks.Queries;

public record GetCompletedTasksQuery() : IRequest<List<OrderShowDto>>;

public class GetCompletedTasksQueryHandler : IRequestHandler<GetCompletedTasksQuery, List<OrderShowDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly ISharedIdentityService _sharedIdentityService;
    public GetCompletedTasksQueryHandler(IOrderDbContext context, IMapper mapper, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _mapper = mapper;
        _sharedIdentityService = sharedIdentityService;
    }
    public async Task<List<OrderShowDto>> Handle(GetCompletedTasksQuery request, CancellationToken cancellationToken)
    {
        var warehousemanId = _sharedIdentityService.GetUserId;
        var tasks = await _context.Orders
            .Include(o => o.Products)
            .Include(o => o.Status)
            .Where(o => o.ClosedBy == warehousemanId)
            .ToListAsync(cancellationToken);
        return _mapper.Map<List<OrderShowDto>>(tasks);
    }
}
