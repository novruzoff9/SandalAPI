namespace Order.Application.Features.Orders.Commands.EditOrderCommand;

public record EditOrderCommand(string Id) : IRequest<bool>;

public class EditOrderCommandHandler : IRequestHandler<EditOrderCommand, bool>
{
    private readonly IOrderDbContext _context;

    public EditOrderCommandHandler(IOrderDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(EditOrderCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NotFound(request.Id, nameof(request));

        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (order == null) { return false; }

        _context.Orders.Update(order);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

