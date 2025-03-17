using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Commands.CompleteOrderCommand;

public record CompleteOrderCommand(string orderId,
    Dictionary<string, int> Products) : IRequest<bool>;

public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommand, bool>
{
    private readonly IOrderDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;
    public CompleteOrderCommandHandler(IOrderDbContext context, ISharedIdentityService sharedIdentityService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
    }
    public async Task<bool> Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(CompleteOrderCommand));
        string userId = _sharedIdentityService.GetUserId;
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == request.orderId, cancellationToken);

        if (order == null) { return false; }

        foreach (var product in order.Products)
        {
            var orderItem = request.Products.FirstOrDefault(x => x.Key == product.ProductId);
            if (orderItem.Equals(default(KeyValuePair<string, int>))) { return false; }
            if (product.Quantity != orderItem.Value) { return false; }
        }

        order.Close(userId);
        _context.Orders.Update(order);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
