using AutoMapper;
using AutoMapper.QueryableExtensions;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using IdentityServer.Protos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Common.Services;
using Order.Application.DTOs.Order;
using Order.Application.Features.Orders.Commands.CompleteOrderCommand;
using Order.Application.Features.Orders.Commands.CreateOrderCommand;
using Order.Application.Features.Orders.Commands.DeleteOrderCommand;
using Order.Application.Features.Orders.Queries;
using Order.Application.Features.Orders.Queries.GetOrderQuery;
using Order.Application.Features.Orders.Queries.GetOrdersByWarehouseQuery;
using Order.Application.Features.Orders.Queries.GetOrdersQuery;
using Shared.Interceptors;
using Shared.ResultTypes;

namespace Order.WebAPI.Controllers;

public class BaseController : ControllerBase
{
    private IMediator _mediator;
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
    private IMapper _mapper;
    protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetService<IMapper>();
}


[Route("api/[controller]")]
[ApiController]
public class OrderController : BaseController
{
    private readonly IExcelService _excelService;
    private readonly IConfiguration _configuration;
    private readonly Identity.IdentityClient _identityClient;
    private readonly CustomerService _customerService;

    public OrderController(IExcelService excelService, IConfiguration configuration, CustomerService customerService)
    {
        _excelService = excelService;
        _configuration = configuration;
        _customerService = customerService;
        string identityGrpcService = _configuration["Services:IdentityGrpcService"] ?? "http://localhost:5003";

        var channel = GrpcChannel.ForAddress($"{identityGrpcService}", new GrpcChannelOptions
        {
            Credentials = ChannelCredentials.Insecure
        });
        var callInvoker = channel.Intercept(new InternalRequestInterceptor());
        _identityClient = new Identity.IdentityClient(callInvoker);
    }

    [HttpGet("{id}/status")]
    public async Task<IActionResult> GetStatusofOrder(string id)
    {
        var order = await Mediator.Send(new GetOrderQuery(id));
        return Ok(order.Status.Name);
    }

    [HttpGet("company")]
    public async Task<IActionResult> GetByCompany()
    {
        var orders = await Mediator.Send(new GetOrdersQuery());
        return Ok(orders);
    }

    [HttpGet("warehouse")]
    public async Task<IActionResult> GetByWarehouse(string? warehouseId)
    {
        var orders = await Mediator.Send(new GetOrdersByWarehouseQuery(warehouseId));
        return Ok(orders);
    }

    [HttpGet("ByStatus/{status}")]
    public async Task<IActionResult> GetByStatus(string status)
    {
        var orders = await Mediator.Send(new GetOrdersByStatusQuery(status));
        var orderDtos = orders.AsQueryable()
            .ProjectTo<OrderShowDto>(Mapper.ConfigurationProvider).ToList();
        foreach (var item in orderDtos)
        {
            item.Customer = await _customerService.GetCustomerFullName(item.Customer);
        }
        var response = Response<List<OrderShowDto>>.Success(orderDtos, 200);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var order = await Mediator.Send(new GetOrderQuery(id));
        GetEmployeeResponse openedByResponse = await _identityClient.GetEmployeeAsync(new GetEmployeeRequest
        {
            Id = order.OpenedBy
        });

        var orderDto = Mapper.Map<OrderShowDto>(order);

        orderDto.OpenedBy = openedByResponse.Employee.Name;
        if (order.ClosedBy != null)
        {
            GetEmployeeResponse closedByResponse = await _identityClient.GetEmployeeAsync(new GetEmployeeRequest
            {
                Id = order.ClosedBy
            });
            orderDto.ClosedBy = closedByResponse.Employee.Name;
        }
        orderDto.Customer = await _customerService.GetCustomerFullName(order.CustomerId);
        return Ok(orderDto);
    }

    [HttpGet("{id}/products")]
    public async Task<IActionResult> GetProductsById(string id)
    {
        var order = await Mediator.Send(new GetOrderQuery(id));

        var orderProducts = order?.Products!.Select(x => new OrderItemShowDto
        {
            Id = x.ProductId,
            Quantity = x.Quantity,
            Name = x.ProductName,
            UnitPrice = x.UnitPrice
        }).ToList();

        return Ok(orderProducts);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> Close(string id, CompleteOrderRequest request)
    {
        var result = await Mediator.Send(new CompleteOrderCommand(id, request.Products));
        return Ok(result);
    }

    [HttpGet("monthly-sales")]
    public async Task<IActionResult> GetMonthlySales()
    {
        var orders = await Mediator.Send(new GetOrdersByStatusQuery("shipped"));

        var monthlySales = orders
            .GroupBy(x => new DateOnly(x.Closed.Value.Year, x.Closed.Value.Month, 1))
            .Select(g => new
            {
                Month = g.Key,
                TotalSales = g.Sum(x => x.Products.Sum(p => p.Quantity))
            })
            .OrderBy(x => x.Month).ToList();

        return Ok(monthlySales);
    }

    [HttpPost("export-file")]
    //public async Task<IActionResult> ExportProducts(DateTimePeriod period)
    public async Task<IActionResult> ExportProducts()
    {
        var orders = await Mediator.Send(new GetOrdersQuery());

        var detailedOrders = orders.Select(x => new
        {
            Warehouse = x.WarehouseName ?? "Unknown Warehouse",
            Opened = x.Opened,
            Closed = x.Closed,
            Products = x.Products != null
                ? string.Join(", ", x.Products
                    .Select(p => $"{p.ProductName} ({p.Quantity})"))
                : "No Products"
        }).ToList();

        byte[] fileContent = await _excelService.ExportToExcel(detailedOrders);
        return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "orders.xlsx");
    }

    [HttpGet("monthly-sales-in-out-come")]
    public async Task<IActionResult> GetMonthlySalesInOutCome()
    {
        var orders = await Mediator.Send(new GetOrdersByStatusQuery("shipped"));

        var monthlySales = orders
            .GroupBy(x => new DateOnly(x.Closed.Value.Year, x.Closed.Value.Month, 1)).ToList();
        var monthlySalesInOutCome = monthlySales.Select(g => new
        {
            Month = g.Key,
            TotalSales = g.Sum(x => x.Products.Sum(p => p.Quantity)),
            TotalIncome = g.Sum(x => x.Products.Sum(p => p.UnitPrice * p.Quantity))
        })
            .OrderBy(x => x.Month).ToList();
        return Ok(monthlySalesInOutCome);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await Mediator.Send(new DeleteOrderCommand(id));
        return Ok(result);
    }
}
