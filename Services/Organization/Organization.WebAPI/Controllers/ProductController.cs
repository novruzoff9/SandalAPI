using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Organization.Application.Products.Commands.CreateProductCommand;
using Organization.Application.Products.Commands.DeleteProductCommand;
using Organization.Application.Products.Commands.EditProductCommand;
using Organization.Application.Products.Queries.GetProductQuery;
using Organization.Application.Products.Queries.GetProductsQuery;
using Organization.Domain.Entities;
using Shared.ResultTypes;

namespace Organization.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var products = await Mediator.Send(new GetProducts());

        var response = Response<List<Product>>.Success(products, 200);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var product = await Mediator.Send(new GetProduct(id));
        var response = Response<Product>.Success(product, 200);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProduct command)
    {
        var product = await Mediator.Send(command);
        return Ok(product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, EditProduct command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }
        var product = await Mediator.Send(command);
        return Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var product = await Mediator.Send(new DeleteProduct(id));
        return Ok(product);
    }
}
