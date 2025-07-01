
namespace Order.Application.Features.Tasks.Commands.CompleteTaskCommand;

public record CompleteTaskCommand(string TaskId, Dictionary<string, int> ProductQuantites) : IRequest<bool>;

public class CompleteTaskCommandHandler : IRequestHandler<CompleteTaskCommand, bool>
{
    private readonly IOrderDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;

    public CompleteTaskCommandHandler(IOrderDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<bool> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = _context.Orders
            .Include(o => o.Status)
            .FirstOrDefault(o => o.Id == request.TaskId);

        if (task is null)
        {
            throw new Shared.Exceptions.NotFoundException("Task not found");
        }
        if (!task.Status!.Equals(OrderStatus.InProgress))
        {
            throw new Exception("This task cannot be completed");
        }
        var warehouseId = _sharedIdentityService.GetWarehouseId;
        if (task.WarehouseId != warehouseId)
        {
            throw new UnauthorizedAccessException("You do not have permission to complete this task");
        }
        // TODO: Implement a domain method to mark the task as completed, which will also handle the logic of updating the status to Prepared. Ve bunun 2 condition ustundeki hisseni legv edecem

        // Validate product quantities
        foreach (var productQuantity in request.ProductQuantites)
        {
            var orderItem = task.Products?.FirstOrDefault(p => p.ProductId == productQuantity.Key);
            if (orderItem is null)
            {
                throw new Shared.Exceptions.NotFoundException($"Order item with product ID {productQuantity.Key} not found in task.");
            }
            if(productQuantity.Value < orderItem.Quantity)
            {
                throw new ArgumentException($"Quantity for product {productQuantity.Key} cannot be less than the current quantity in the task.");
            }
        }

        string userId = _sharedIdentityService.GetUserId;
        task.Close(userId);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
