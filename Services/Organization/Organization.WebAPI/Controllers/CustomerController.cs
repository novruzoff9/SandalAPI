using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Organization.Application.Customers.Commands.CreateCustomerCommand;
using Organization.Application.Customers.Commands.DeleteCustomerCommand;
using Organization.Application.Customers.Commands.EditCustomerCommand;
using Organization.Application.Customers.Queries.GetCustomerQuery;
using Organization.Application.Customers.Queries.GetCustomersQuery;

namespace Organization.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var customers = await Mediator.Send(new GetCustomersQuery());
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var customer = await Mediator.Send(new GetCustomerQuery(id));
        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateCustomerCommand command)
    {
        var customer = await Mediator.Send(command);
        return Ok(customer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, EditCustomerContactCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }
        var customer = await Mediator.Send(command);
        return Ok(customer);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await Mediator.Send(new DeleteCustomerCommand(id));
        return NoContent();
    }
}
