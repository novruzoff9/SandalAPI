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
            .Include(x => x.Status)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
        if (task is null)
        {
            throw new Shared.Exceptions.NotFoundException("Task not found");
        }

        // Fix for CS9135: Use explicit comparison with Id property of OrderStatus  
        if (task.Status?.Id != OrderStatus.StockConfirmed.Id && task.Status?.Id != OrderStatus.InProgress.Id)
        {
            throw new Exception("This task cannot prepare");
        }

        var warehouseId = _sharedIdentityService.GetWarehouseId;
        if (task.WarehouseId != warehouseId)
        {
            throw new UnauthorizedAccessException("You do not have permission to view this task");
        }

        var taskDto = _mapper.Map<OrderShowDto>(task);
        taskDto.Customer = await _customerService.GetCustomerFullNameAsync(task.CustomerId);
        taskDto.OpenedBy = await _identityGrpcClient.GetUserFullNameAsync(task.OpenedBy);

        foreach (string shelfOfProduct in task.Note.Split('\n'))
        {
            int colonIndex = shelfOfProduct.IndexOf(':');
            if (colonIndex > 0)
            {
                string productId = shelfOfProduct.Substring(0, colonIndex - 1).Trim();
                string shelfCode = shelfOfProduct.Substring(colonIndex + 1 + 1).Trim();
                var orderItem = task.Products?.FirstOrDefault(p => p.ProductId == productId);
                if (orderItem is null)
                {
                    throw new Shared.Exceptions.NotFoundException($"Order item with product ID {productId} not found in task.");
                }
                var orderDtoItem = taskDto.Products?.FirstOrDefault(p => p.Id == orderItem.Id);
                if (orderDtoItem != null)
                {
                    orderDtoItem.ShelfCode = shelfCode;
                }
            }
        }

        return taskDto;
    }
}
