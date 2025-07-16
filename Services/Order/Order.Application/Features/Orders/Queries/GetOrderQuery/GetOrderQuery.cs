namespace Order.Application.Features.Orders.Queries.GetOrderQuery;

public record GetOrderQuery(string Id) : IRequest<OrderShowDto>;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderShowDto>
{
    private readonly IMapper _mapper;
    private readonly IOrderDbContext _context;
    private readonly ICustomerService _customerService;
    private readonly IIdentityGrpcClient _identityGrpcClient;
    private readonly ISharedIdentityService _sharedIdentityService;

    public GetOrderQueryHandler(IOrderDbContext context, ISharedIdentityService sharedIdentityService, IMapper mapper, ICustomerService customerService, IIdentityGrpcClient identityGrpcClient)
    {
        _mapper = mapper;
        _context = context;
        _customerService = customerService;
        _identityGrpcClient = identityGrpcClient;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<OrderShowDto> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        var order = await _context.Orders
            .Include(x => x.Products)
            .Include(x => x.Status)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (order == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Order), request.Id);
        }

        var orderDto = _mapper.Map<OrderShowDto>(order);
        orderDto.Customer = await _customerService.GetCustomerFullNameAsync(order.CustomerId);

        orderDto.OpenedBy = await _identityGrpcClient.GetUserFullNameAsync(order.OpenedBy);

        if (order.Closed.HasValue) 
        {
            orderDto.ClosedBy = await _identityGrpcClient.GetUserFullNameAsync(order.ClosedBy!);
        }
        //foreach (string shelfOfProduct in orderDto.Note.Split('\n'))
        //{
        //    int colonIndex = shelfOfProduct.IndexOf(':');
        //    if (colonIndex > 0)
        //    {
        //        string productId = shelfOfProduct.Substring(0, colonIndex - 1).Trim();
        //        string shelfCode = shelfOfProduct.Substring(colonIndex + 1 + 1).Trim();
        //        var orderItem = orderDto.Products?.FirstOrDefault(p => p.ProductId == productId);
        //        if (orderItem is null)
        //        {
        //            throw new Shared.Exceptions.NotFoundException($"Order item with product ID {productId} not found in task.");
        //        }
        //        var orderDtoItem = orderDto.Products?.FirstOrDefault(p => p.Id == orderItem.Id);
        //        if (orderDtoItem != null)
        //        {
        //            orderDtoItem.ShelfCode = shelfCode;
        //        }
        //    }
        //}

        return orderDto;
    }
}

