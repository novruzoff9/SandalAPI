using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Organization.Application.Common.Services;
using Organization.Application.Products.Commands.CreateProductCommand;
using Organization.Application.Products.Commands.DeleteProductCommand;
using Organization.Application.Products.Commands.EditProductCommand;
using Organization.Application.Products.Queries.GetProductQuery;
using Organization.Application.Products.Queries.GetProductsQuery;
using Organization.Domain.Entities;
using Organization.WebAPI.DTOs.General;
using Organization.WebAPI.DTOs.Product;
using Shared.ResultTypes;
using Shared.Services;

namespace Organization.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : BaseController
{
    private readonly IExcelService _excelService;
    private readonly ISharedIdentityService _sharedIdentityService;

    public ProductController(IExcelService excelService, ISharedIdentityService sharedIdentityService)
    {
        _excelService = excelService;
        _sharedIdentityService = sharedIdentityService;
    }

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

    [HttpPost("import-file")]
    public async Task<IActionResult> ImportProducts(IFormFile file)
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        var products = await _excelService.ImportProducts(file, companyId);
        foreach (var product in products)
        {
            await Mediator.Send(new CreateProduct
            (
                Name: product.Name,
                Description: product.Description,
                PurchasePrice: product.PurchasePrice,
                SellPrice: product.SellPrice,
                Quantity: product.Quantity,
                ImageUrl: product.ImageUrl
            ));
        }
        return Ok(products);
    }

    [HttpGet("export-file")]
    public async Task<IActionResult> ExportProducts([FromBody] DateTimePeriod period)
    {
        var products = await Mediator.Send(new GetProductsWithOrder(period.Start, period.End));
        var detailedProducts = products.Select(x => new ExportProductDto
        {
            Name = x.Name,
            Description = x.Description,
            PurchasePrice = x.PurchasePrice,
            SellPrice = x.SellPrice,
            Quantity = x.Quantity,
            Ordered = x.OrderItems.Sum(x=>x.Quantity)
        });
        string path = await _excelService.ExportToExcel(detailedProducts);
        return Ok(path);
    }
}
