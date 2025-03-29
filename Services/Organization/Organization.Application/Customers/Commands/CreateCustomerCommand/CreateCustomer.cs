
namespace Organization.Application.Customers.Commands.CreateCustomerCommand;

public record CreateCustomerCommand(string FirstName, string LastName, string Email, string Phone) : IRequest<bool>;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _identityService;

    public CreateCustomerCommandHandler(IApplicationDbContext context, ISharedIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<bool> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(CreateCustomerCommand));
        string companyId = _identityService.GetCompanyId;

        var customer = new Customer(request.FirstName, request.LastName, request.Email, request.Phone, companyId);

        await _context.Customers.AddAsync(customer, cancellationToken);

        var success = await _context.SaveChangesAsync(cancellationToken) > 0;

        return success;
    }
}

