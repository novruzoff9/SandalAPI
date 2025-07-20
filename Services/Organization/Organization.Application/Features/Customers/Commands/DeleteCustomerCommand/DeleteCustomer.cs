namespace Organization.Application.Features.Customers.Commands.DeleteCustomerCommand;

public record DeleteCustomerCommand(string Id) : IRequest<bool>;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteCustomerCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request.Id, nameof(request.Id));

        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (customer == null) { return false; }
        _context.Customers.Remove(customer);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

