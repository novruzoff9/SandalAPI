using Organization.Domain.ValueObjects;

namespace Organization.Application.Features.Customers.Commands.EditCustomerCommand;

public record EditCustomerContactCommand(string Id, string email, string phone, Address Address) : IRequest<bool>;

public class EditCustomerContactCommandHandler : IRequestHandler<EditCustomerContactCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public EditCustomerContactCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(EditCustomerContactCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NotFound(request.Id, nameof(request));

        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (customer == null) { return false; }

        customer.UpdateContactInfo(request.email, request.phone);

        customer.UpdateAddress(request.Address);

        _context.Customers.Update(customer);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

