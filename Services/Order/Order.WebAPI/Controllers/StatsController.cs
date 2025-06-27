using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.Orders.Queries.GetOrdersQuery;
using Order.Application.Features.Orders.Queries;

namespace Order.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatsController : BaseController
{
    private readonly IExcelService _excelService;
    public StatsController(IExcelService excelService)
    {
        _excelService = excelService;
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
            Warehouse = x.Warehouse ?? "Unknown Warehouse",
            Opened = x.Opened,
            Closed = x.Closed,
            Products = x.Products != null
                ? string.Join(", ", x.Products
                    .Select(p => $"{p.Name} ({p.Quantity})"))
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

    [HttpGet("top5Products")]
    public async Task<IActionResult> GetTop5Products()
    {
        var orders = await Mediator.Send(new GetOrdersQuery());
        var top5Items = orders
            .SelectMany(o => o.Products)
            .GroupBy(p => p)
            .Select(g => new { Product = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .Select(x => x.Product)
            .ToList();
        return Ok(top5Items);
    }

}
