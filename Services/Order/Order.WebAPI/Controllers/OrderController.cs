using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.Orders.Commands.CreateOrderCommand;
using Order.Application.Features.Orders.Commands.DeleteOrderCommand;
using Order.Application.Features.Orders.Commands.RetryOrderCommand;
using Order.Application.Features.Orders.Queries;
using Order.Application.Features.Orders.Queries.GetOrderQuery;
using Order.Application.Features.Orders.Queries.GetOrdersByWarehouseQuery;
using Order.Application.Features.Orders.Queries.GetOrdersQuery;
using Shared.DTOs.Export;
using Shared.DTOs.General;

namespace Order.WebAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
public class OrderController : BaseController
{
    [HttpGet("{id}/status")]
    public async Task<IActionResult> GetStatusofOrder(string id)
    {
        var order = await Mediator.Send(new GetOrderQuery(id));
        return Ok(order.Status);
    }

    [HttpGet("company")]
    public async Task<IActionResult> GetByCompany()
    {
        var orders = await Mediator.Send(new GetOrdersQuery());
        var reponse = Response<List<OrderShowDto>>.Success(orders,200);
        return Ok(reponse);
    }

    [HttpGet("warehouse")]
    public async Task<IActionResult> GetByWarehouse(string? warehouseId)
    {
        var orders = await Mediator.Send(new GetOrdersByWarehouseQuery(warehouseId));
        var response = Response<List<OrderShowDto>>.Success(orders, 200);
        return Ok(response);
    }

    [HttpGet("ByStatus/{status}")]
    public async Task<IActionResult> GetByStatus(string status)
    {
        var orders = await Mediator.Send(new GetOrdersByStatusQuery(status));
        var response = Response<List<OrderShowDto>>.Success(orders, 200);
        return Ok(response);
    }

    [HttpGet("ByCustomer/{id}")]
    public async Task<IActionResult> GetByCustomer(string id)
    {
        var orders = await Mediator.Send(new GetOrdersByCustomerQuery(id));
        var response = Response<List<OrderShowDto>>.Success(orders, 200);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var order = await Mediator.Send(new GetOrderQuery(id));
        var response = Response<OrderShowDto>.Success(order, 200);
        return Ok(response);
    }

    [HttpGet("{id}/products")]
    public async Task<IActionResult> GetProductsById(string id)
    {
        var order = await Mediator.Send(new GetOrderQuery(id));
        return Ok(order.Products);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/retry")]
    public async Task<IActionResult> Retry(string id)
    {
        var result = await Mediator.Send(new RetryOrderCommand(id));
        return Ok(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await Mediator.Send(new DeleteOrderCommand(id));
        return Ok(result);
    }

    [HttpPost("export-data")]
    public async Task<IActionResult> ExportData(DateTimePeriod period)
    {
        var orders = await Mediator.Send(new GetOrdersQuery(x=>x.Opened >= period.Start && x.Opened <= period.End));
        var detailedOrders = orders.Select(o => new ExportOrderDto
        {
            Id = o.Id,
            OrderDate = o.Opened,
            CustomerName = o.Customer,
            WarehouseName = o.Warehouse,
            Status = o.Status,
            TotalProduct = o.Quantity,
            TotalPrice = o.TotalPrice,
        }).ToList();
        var response = Response<List<ExportOrderDto>>.Success(detailedOrders, 200);
        return Ok(response);
    }
}
