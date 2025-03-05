using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Organization.Application.Common.Services;
using Organization.Application.Products.Commands.CreateProductCommand;
using Organization.Application.Shelves.Queries.GetShelvesQuery;
using Organization.Application.Warehouses.Commands.CreateWarehouseCommand;
using Organization.Application.Warehouses.Commands.DeleteWarehouseCommand;
using Organization.Application.Warehouses.Commands.EditWarehouseCommand;
using Organization.Application.Warehouses.Queries.GetWarehouseQuery;
using Organization.Application.Warehouses.Queries.GetWarehousesQuery;
using Organization.Domain.Entities;
using Shared.ResultTypes;
using Shared.Services;

namespace Organization.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : BaseController
{
    private readonly ISharedIdentityService _sharedIdentityService;
    private readonly IExcelService _excelService;

    public WarehouseController(ISharedIdentityService sharedIdentityService, IExcelService excelService)
    {
        _sharedIdentityService = sharedIdentityService;
        _excelService = excelService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var response = await Mediator.Send(new GetWarehouses());
        var result = Response<List<Warehouse>>.Success(response, 200);
        return Ok(result);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var response = await Mediator.Send(new GetWarehouse(id));
        return Ok(response);
    }

    [HttpGet("company/{id}")]
    public async Task<IActionResult> GetByCompany(string id)
    {
        var response = await Mediator.Send(new GetWarehousesByCompany(id));
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateWarehouse command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpGet("occupancy-rate")]
    public async Task<IActionResult> GetOccupancyRate()
    {
        var warehouses = await Mediator.Send(new GetWarehouses());
        var warehouse = warehouses.FirstOrDefault();
        var shelves = await Mediator.Send(new GetShelvesByWarehouse(warehouse.Id));
        decimal emptySheleves = shelves.Where(x => x.ShelfProducts.Count == 0).Count();
        decimal totalShelves = shelves.Count;
        decimal fullShelves = totalShelves - emptySheleves;
        decimal occupancyRate = fullShelves / totalShelves;
        return Ok(occupancyRate);
    }

    [HttpPost("import-file")]
    public async Task<IActionResult> ImportWarehouses(IFormFile file)
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        var warehouses = await _excelService.ImportWarehouses(file, companyId);
        foreach (var warehouse in warehouses)
        {
            await Mediator.Send(new CreateWarehouse
            (
                Name: warehouse.Name,
                GoogleMaps: warehouse.GoogleMaps,
                City: warehouse.City,
                State: warehouse.State,
                Street: warehouse.Street,
                ZipCode: warehouse.ZipCode
            ));
        }
        var response = Response<NoContent>.Success(200);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, EditWarehouse command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var response = await Mediator.Send(new DeleteWarehouse(id));
        return Ok(response);
    }
}
