namespace Order.Application.Features.Orders.Commands.DeleteOrderCommand;

public record DeleteOrderCommand(string Id) : IRequest<bool>;

public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool>
{
    private readonly IOrderDbContext _context;

    public DeleteOrderCommandHandler(IOrderDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request.Id, nameof(request.Id));

        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (order == null) { return false; }
        _context.Orders.Remove(order);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

