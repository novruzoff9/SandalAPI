using Microsoft.AspNetCore.Mvc;
using Organization.Application.Common.Models.Shelf;
using Organization.Application.Common.Models.ShelfProducts;
using Organization.Application.Shelves.Commands.AddProductsToShelf;
using Organization.Application.Shelves.Commands.CreateShelfCommand;
using Organization.Application.Shelves.Commands.DeleteShelfCommand;
using Organization.Application.Shelves.Commands.EditShelfCommand;
using Organization.Application.Shelves.Commands.RemoveProductsFromShelf;
using Organization.Application.Shelves.Queries.GetShelfQuery;
using Organization.Application.Shelves.Queries.GetShelvesQuery;
using Organization.Domain.Entities;
using Shared.ResultTypes;

namespace Organization.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShelfController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var shelves = await Mediator.Send(new GetShelves());
        var response = Response<List<ShelfDTO>>.Success(shelves, 200);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var data = await Mediator.Send(new GetShelf(id));
        var response = Response<Shelf>.Success(data, 200);
        return Ok(response);
    }

    [HttpGet("{code}/products")]
    public async Task<IActionResult> GetProductsOfSehlf(string code)
    {
        var shelves = await Mediator.Send(new GetShelves());
        var shelf = shelves.FirstOrDefault(x => x.Code == code);
        var shelfProducts = await Mediator.Send(new GetProductsByShelf(shelf.Id));
        var response = Response<List<ShelfProduct>>.Success(shelfProducts, 200);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateShelf command)
    {
        var result = await Mediator.Send(command);
        var response = Response<bool>.Success(result, 201);
        return Ok(response);
    }

    [HttpPost("products")]
    public async Task<IActionResult> AddProductToShelf(AddingProductsToShelfDTO request)
    {
        var shelves = await Mediator.Send(new GetShelves());
        var shelf = shelves.FirstOrDefault(x => x.Code == request.ShelfCode);
        var result = await Mediator.Send(new AddProductsToShelf(shelf.Id, request.ProductIds));
        var response = Response<NoContent>.Success(204);
        return Ok(response);
    }

    [HttpPost("products/remove")]
    public async Task<IActionResult> RemoveProductFromShelf(RemoveProductsFromShelfDTO request)
    {
        var shelves = await Mediator.Send(new GetShelves());
        var shelf = shelves.FirstOrDefault(x => x.Code == request.ShelfCode);
        var result = await Mediator.Send(new RemoveProductsFromShelf(request.ShelfCode, request.Products));
        var response = Response<NoContent>.Success(204);
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
        var result = await Mediator.Send(new DeleteShelf(id));
        var response = Response<NoContent>.Success(204);
        return Ok(response);
    }

    [HttpGet("warehouse/{id}")]
    public async Task<IActionResult> GetByWarehouse(string id)
    {
        var data = await Mediator.Send(new GetShelvesByWarehouse(id));
        var response = Response<List<ShelfDTO>>.Success(data, 200);
        return Ok(response);
    }
}
