using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organization.Application.Common.Services;
using Organization.Application.Products.Queries.GetProductsQuery;
using Organization.Domain.Entities;
using Organization.Infrastructure.Data;
using Organization.WebAPI.DTOs.General;
using Organization.WebAPI.DTOs.Order;
using Shared.ResultTypes;
using Shared.Services;

namespace Organization.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ISharedIdentityService _sharedIdentityService;
    private readonly IExcelService _excelService;

    public OrderController(ApplicationDbContext dbContext, ISharedIdentityService sharedIdentityService, IExcelService excelService)
    {
        _dbContext = dbContext;
        _sharedIdentityService = sharedIdentityService;
        _excelService = excelService;
    }

    [HttpGet("company")]
    public async Task<IActionResult> GetByCompany()
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        var orders = await _dbContext.Orders.Where(x => x.CompanyId == companyId).ToListAsync();
        return Ok(orders);
    }

    [HttpGet("warehouse")]
    public async Task<IActionResult> GetByWarehouse()
    {
        string warehouseId = _sharedIdentityService.GetWarehouseId;
        var orders = await _dbContext.Orders.Where(x => x.WarehouseId == warehouseId).ToListAsync();
        return Ok(orders);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveOrders()
    {
        //string companyId = _sharedIdentityService.GetCompanyId;
        var orders = await _dbContext.Orders.Include(x => x.Warehouse)
            .Select(x => new OrderShowDto
            {
                Id = x.Id,
                Warehouse = x.Warehouse.Name,
                Opened = x.Opened,
                Closed = x.Closed,
                Status = "gozleyir"
            })
            .Where(x => x.Closed == null).ToListAsync();
        var response = Response<List<OrderShowDto>>.Success(orders, 200);
        return Ok(response);
    }

    [HttpGet("completed")]
    public async Task<IActionResult> GetCompletedOrders()
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        var orders = await _dbContext.Orders.Include(x => x.Warehouse)
            .Select(x => new OrderShowDto
            {
                Id = x.Id,
                Warehouse = x.Warehouse.Name,
                Opened = x.Opened,
                Closed = x.Closed,
                Status = "Hazir"
            })
            .Where(x => x.Closed != null).ToListAsync();
        var response = Response<List<OrderShowDto>>.Success(orders, 200);
        return Ok(response);
    }

    [HttpGet("{id}/products")]
    public async Task<IActionResult> GetById(string id)
    {
        var order = await _dbContext.Orders.Include(x => x.Products)
            .ThenInclude(x => x.Product)
            .ThenInclude(x => x.ShelfProducts)
            .ThenInclude(x => x.Shelf)
            .FirstOrDefaultAsync(x => x.Id == id);

        var orderProducts = order.Products.Select(x => new OrderItemShowDto
        {
            ProductId = x.ProductId,
            Quantity = x.Quantity,
            ProductName = x.Product.Name
            //ShelfCode = x.Product.ShelfProducts.FirstOrDefault().Shelf.Code
        }).ToList();

        string warehouseId = _sharedIdentityService.GetWarehouseId;
        if (order.WarehouseId != warehouseId)
        {
            return Unauthorized();
        }
        var shelves = await _dbContext.Shelves.Where(x => x.WarehouseID == warehouseId)
            .Include(x => x.ShelfProducts)
            .ThenInclude(x => x.Product).ToListAsync();


        return Ok(orderProducts);
    }

    [HttpPost]
    public async Task<IActionResult> Create(OrderDto order)
    {
        var companyId = _sharedIdentityService.GetCompanyId;

        Order newOrder = new()
        {
            Id = Guid.NewGuid().ToString(),
            WarehouseId = order.WarehouseId,
            CompanyId = companyId,
            Products = order.OrderItems.Select(x => new OrderItem
            {
                Id = Guid.NewGuid().ToString(),
                ProductId = x.ProductId,
                Quantity = x.Quantity
            }).ToList(),
            Opened = DateTime.Now
        };
        _dbContext.Orders.Add(newOrder);
        await _dbContext.SaveChangesAsync();
        return Ok(order);
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> Close(string id, List<Product> products)
    {
        var order = await _dbContext.Orders.Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == id);
        if (order == null)
        {
            return NotFound();
        }
        foreach (var product in order.Products)
        {
            var orderItem = products.FirstOrDefault(x => x.Id == product.Id);
            if (orderItem == null || orderItem.Quantity != product.Quantity)
            {
                return BadRequest();
            }

        }
        order.Closed = DateTime.Now;
        await _dbContext.SaveChangesAsync();
        return Ok("success");
    }

    [HttpGet("monthly-sales")]
    public async Task<IActionResult> GetMonthlySales()
    {
        var companyId = _sharedIdentityService.GetCompanyId;
        var orders = await _dbContext.Orders.Include(x => x.Products)
            .Where(x => x.CompanyId == companyId && x.Closed != null)
            .ToListAsync();

        var monthlySales = orders.GroupBy(x => x.Closed.Value.Month)
            .Select(g => new
            {
                Month = g.Key,
                TotalSales = g.Sum(x => x.Products.Sum(p => p.Quantity))
            }).ToList();

        return Ok(monthlySales);
    }

    [HttpGet("export-file")]
    public async Task<IActionResult> ExportProducts(DateTimePeriod period)
    {
        var orders = await _dbContext.Orders
            .Include(x => x.Products)
            .ThenInclude(x => x.Product)
            .Where(x => x.Opened >= period.Start && x.Opened <= period.End)
            .ToListAsync();

        var detailedOrders = orders.Select(x => new
        {
            Warehouse = x.Warehouse?.Name ?? "Unknown Warehouse", // x.Warehouse null ise
            Opened = x.Opened,
            Closed = x.Closed,
            Products = x.Products != null
                ? string.Join(", ", x.Products
                    .Where(p => p.Product != null) 
                    .Select(p => $"{p.Product.Name ?? "Unnamed Product"} ({p.Quantity})"))
        :       "No Products" 
        });
        string path = await _excelService.ExportToExcel(detailedOrders);
        return Ok(path);
    }
}
