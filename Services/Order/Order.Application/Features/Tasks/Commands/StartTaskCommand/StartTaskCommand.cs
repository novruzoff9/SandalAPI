namespace Order.Application.Features.Tasks.Commands.StartTaskCommand;

public record StartTaskCommand(string TaskId) : IRequest<bool>;

public class StartTaskCommandHandler : IRequestHandler<StartTaskCommand, bool>
{
    private readonly IOrderDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;
    public StartTaskCommandHandler(IOrderDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }
    public async Task<bool> Handle(StartTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.Orders
            .Include(o => o.Status)
            .FirstOrDefaultAsync(o => o.Id == request.TaskId, cancellationToken);
        if (task is null)
        {
            throw new Shared.Exceptions.NotFoundException("Task not found");
        }
        if (!task.Status!.Equals(OrderStatus.StockConfirmed))
        {
            throw new Exception("This task cannot be started");
        }
        var warehouseId = _sharedIdentityService.GetWarehouseId;
        if (task.WarehouseId != warehouseId)
        {
            throw new UnauthorizedAccessException("You do not have permission to start this task");
        }

        //TODO: Demeli task-i InProgress elemek ucun bir domain metodu olacag. Kim terefendinde ve ne vaxt basladildigini ozunde saxlayacag. ve StockConfirmed statusundan InProgress statusuna kecid edecek. Amma eger order-in statusu StockConfirmed deyilse exception verecek
        task.UpdateStatus(OrderStatus.InProgress);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
