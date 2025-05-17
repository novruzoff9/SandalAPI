using EventBus.Base.Abstraction;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Commands.RetryOrderCommand;

public record RetryOrderCommand(string Id) : IRequest<bool>;

public class RetryOrderCommandHandler : IRequestHandler<RetryOrderCommand, bool>
{
    private readonly IOrderDbContext _context;
    private readonly IEventBus _eventBus;

    public RetryOrderCommandHandler(IOrderDbContext context, IEventBus eventBus)
    {
        _context = context;
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(RetryOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.Include(x=>x.Products)
            .FirstOrDefaultAsync(x => x.Id == request.Id);
        if (order == null) return false;
        OrderCreatedIntegrationEvent orderCreatedIntegrationEvent = new(request.Id, order.CustomerId, order.WarehouseId, order.Products.Select(p => new OrderItemDto
        {
            ProductId = p.ProductId,
            Quantity = p.Quantity
        }).ToList());

        _eventBus.Publish(orderCreatedIntegrationEvent);
        return true;
    }
}