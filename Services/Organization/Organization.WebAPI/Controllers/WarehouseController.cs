using Microsoft.AspNetCore.Mvc;
using Organization.Application.Common.Services;
using Organization.Application.DTOs.Warehouse;
using Organization.Application.Features.Warehouses.Commands.CreateWarehouseCommand;
using Organization.Application.Features.Warehouses.Commands.DeleteWarehouseCommand;
using Organization.Application.Features.Warehouses.Commands.EditWarehouseCommand;
using Organization.Application.Features.Warehouses.Queries.GetWarehouseQuery;
using Organization.Application.Features.Warehouses.Queries.GetWarehousesQuery;
using Organization.Domain.Entities;
using Shared.DTOs.Export;
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
        var result = Response<List<WarehouseDto>>.Success(response, 200);
        return Ok(result);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var warehouse = await Mediator.Send(new GetWarehouse(id));
        var response = Response<WarehouseDto>.Success(warehouse, 200);
        return Ok(response);
    }

    [HttpGet("company/{id}")]
    public async Task<IActionResult> GetByCompany(string id)
    {
        var warehouses = await Mediator.Send(new GetWarehousesByCompany(id));
        var response = Response<List<WarehouseDto>>.Success(warehouses, 200);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateWarehouse command)
    {
        var result = await Mediator.Send(command);
        if (result == null)
        {
            return BadRequest();
        }
        var response = Response<Warehouse>.Success(result, 201);
        return Ok(response);
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
        var result = await Mediator.Send(command);
        if (result == false)
        {
            return NotFound();
        }
        var response = Response<bool>.Success(result, 201);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await Mediator.Send(new DeleteWarehouse(id));
        if (result == false)
        {
            return NotFound();
        }
        var response = Response<bool>.Success(result, 204);
        return Ok(response);
    }

    [HttpPost("export-data")]
    public async Task<IActionResult> ExportWarehouses()
    {
        var warehouses = await Mediator.Send(new GetWarehouses());
        var detailedWarehouses = warehouses.Select(w => new ExportWarehouseDto
        {
            Id = w.Id,
            Name = w.Name,
            GoogleMaps = w.GoogleMaps ??= string.Empty,
            City = w.City,
            State = w.State,
            Street = w.Street,
            ZipCode = w.ZipCode,
            Workers = w.EmployeeCount,
            Capacity = w.Shelves
        }).ToList();
        var response = Response<List<ExportWarehouseDto>>.Success(detailedWarehouses, 200);
        return Ok(response);
    }
}
