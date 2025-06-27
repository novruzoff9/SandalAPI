using Shared.Exceptions;

namespace Order.Application.Features.Tasks.Queries;

public record GetTaskDetailsQuery(string Id) : IRequest<OrderShowDto>;

public class GetTaskDetailsQueryHandler : IRequestHandler<GetTaskDetailsQuery, OrderShowDto>
{
    private readonly IMapper _mapper;
    private readonly IOrderDbContext _context;
    private readonly ICustomerService _customerService;
    private readonly IIdentityGrpcClient _identityGrpcClient;
    private readonly ISharedIdentityService _sharedIdentityService;

    public GetTaskDetailsQueryHandler(IOrderDbContext context, IMapper mapper, ISharedIdentityService sharedIdentityService, ICustomerService customerService, IIdentityGrpcClient identityGrpcClient)
    {
        _mapper = mapper;
        _context = context;
        _customerService = customerService;
        _sharedIdentityService = sharedIdentityService;
        _identityGrpcClient = identityGrpcClient;
    }

    public async Task<OrderShowDto> Handle(GetTaskDetailsQuery request, CancellationToken cancellationToken)
    {
        var task = await _context.Orders.Include(o => o.Products)
            .Include(x=>x.Status)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
        if (task == null)
        {
            throw new Shared.Exceptions.NotFoundException("Task not found"); 
        }

        if(!task.Status!.Equals(OrderStatus.StockConfirmed))
        {
            //TODO: This exception should be more specific, e.g., TaskCannotBePreparedException
            throw new Exception("This task cannot preapare");
        }

        var warehouseId = _sharedIdentityService.GetWarehouseId;
        if (task.WarehouseId != warehouseId)
        {
            throw new UnauthorizedAccessException("You do not have permission to view this task");
        }

        var taskDto = _mapper.Map<OrderShowDto>(task);
        taskDto.Customer = await _customerService.GetCustomerFullNameAsync(task.CustomerId);
        taskDto.OpenedBy = await _identityGrpcClient.GetUserFullNameAsync(task.OpenedBy);

        //TODO: Order-in mehsullarinin hansi reflerde oldugunu tap


        return taskDto;
    }
}
