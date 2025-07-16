using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Organization.Application.Customers.Commands.CreateCustomerCommand;
using Organization.Application.Customers.Commands.DeleteCustomerCommand;
using Organization.Application.Customers.Commands.EditCustomerCommand;
using Organization.Application.Customers.Queries.GetCustomerQuery;
using Organization.Application.Customers.Queries.GetCustomersQuery;
using Organization.Domain.Entities;
using Shared.DTOs.Export;
using Shared.ResultTypes;

namespace Organization.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var customers = await Mediator.Send(new GetCustomersQuery());
        var response = Response<List<Customer>>.Success(customers, 200);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var customer = await Mediator.Send(new GetCustomerQuery(id));
        var response = Response<Customer>.Success(customer, 200);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateCustomerCommand command)
    {
        var customer = await Mediator.Send(command);
        var response = Response<bool>.Success(customer, 201);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, EditCustomerContactCommand command)
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
        var result = await Mediator.Send(new DeleteCustomerCommand(id));
        if (result == false)
        {
            return NotFound();
        }
        var response = Response<NoContent>.Success(204);
        return Ok(response);
    }

    [HttpPost("export-data")]
    public async Task<IActionResult> ExportData()
    {
        var customers = await Mediator.Send(new GetCustomersQuery());
        var detailedCustomers = customers.Select(c => new ExportCustomerDto
        {
            Id = c.Id,
            FullName = $"{c.FirstName} {c.LastName}",
            Email = c.Email,
            Phone = c.Phone,
            Address = $"{c.Address.City}, {c.Address.District}, {c.Address.Street}, {c.Address.ZipCode}"
        }).ToList();
        var response = Response<List<ExportCustomerDto>>.Success(detailedCustomers, 200);
        return Ok(response);
    }
}
