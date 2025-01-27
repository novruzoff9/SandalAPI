﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Organization.Application.Warehouses.Commands.CreateWarehouseCommand;
using Organization.Application.Warehouses.Commands.DeleteWarehouseCommand;
using Organization.Application.Warehouses.Commands.EditWarehouseCommand;
using Organization.Application.Warehouses.Queries.GetWarehouseQuery;
using Organization.Application.Warehouses.Queries.GetWarehousesQuery;
using Organization.Domain.Entities;
using Shared.ResultTypes;

namespace Organization.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : BaseController
{
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
