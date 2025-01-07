using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Organization.Application.Shelves.Commands.CreateShelfCommand;
using Organization.Application.Shelves.Commands.DeleteShelfCommand;
using Organization.Application.Shelves.Commands.EditShelfCommand;
using Organization.Application.Shelves.Commands.AddProductsToShelf;
using Organization.Application.Shelves.Queries.GetShelfQuery;
using Organization.Application.Shelves.Queries.GetShelvesQuery;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Organization.Application.Common.Models.ShelfProducts;
using Organization.WebAPI.DTOs.Shelf;

namespace Organization.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShelfController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var response = await Mediator.Send(new GetShelves());
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var response = await Mediator.Send(new GetShelf(id));
        return Ok(response);
    }

    [HttpGet("{code}/products")]
    public async Task<IActionResult> GetProductsOfSehlf(string code)
    {
        var shelves = await Mediator.Send(new GetShelves());
        var shelf = shelves.FirstOrDefault(x => x.Code == code);
        var response = await Mediator.Send(new GetProductsByShelf(shelf.Id));
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateShelf command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("products")]
    public async Task<IActionResult> AddProductToShelf(AddingProductsToShelfDTO request)
    {
        var shelves = await Mediator.Send(new GetShelves());
        var shelf = shelves.FirstOrDefault(x => x.Code == request.ShelfId);
        var response = await Mediator.Send(new AddProductsToShelf(shelf.Id, request.ProductIds));
        return Ok(response);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, EditShelf command)
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
        var response = await Mediator.Send(new DeleteShelf(id));
        return Ok(response);
    }

    [HttpGet("warehouse/{id}")]
    public async Task<IActionResult> GetByWarehouse(string id)
    {
        var response = await Mediator.Send(new GetShelvesByWarehouse(id));
        return Ok(response);
    }
}
