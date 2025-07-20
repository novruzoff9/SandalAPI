using Microsoft.AspNetCore.Mvc;
using Organization.Application.Common.Interfaces;
using Organization.Application.Common.Models.StockHistory;
using Organization.Application.Common.Services;
using Organization.Application.Features.Products.Commands.BulkDecreaseProductsCommand;
using Organization.Application.Features.Products.Commands.BulkIncreaseProductsCommand;
using Organization.Application.Features.Products.Commands.CreateProductCommand;
using Organization.Application.Features.Products.Commands.DeleteProductCommand;
using Organization.Application.Features.Products.Commands.EditProductCommand;
using Organization.Application.Features.Products.Commands.IncreaseProductCommand;
using Organization.Application.Features.Products.Queries.GetProductQuery;
using Organization.Application.Features.Products.Queries.GetProductsQuery;
using Organization.Domain.Entities;
using Shared.DTOs.Export;
using Shared.ResultTypes;
using Shared.Services;

namespace Organization.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : BaseController
{
    private readonly IExcelService _excelService;
    private readonly IStockHistoryService _stockHistoryService;
    private readonly ISharedIdentityService _sharedIdentityService;

    public ProductController(IExcelService excelService, ISharedIdentityService sharedIdentityService, IStockHistoryService stockHistoryService)
    {
        _excelService = excelService;
        _sharedIdentityService = sharedIdentityService;
        _stockHistoryService = stockHistoryService;
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
        var response = Response<bool>.Success(product, 201);
        return Ok(response);
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
        if (!product)
        {
            return BadRequest("Product could not be deleted.");
        }
        var response = Response<bool>.Success(product, 204);
        return Ok(response);
    }

    [HttpPost("{id}/increase")]
    public async Task<IActionResult> IncreaseProduct(string id, [FromBody] int quantity)
    {
        var product = await Mediator.Send(new IncreaseProductCommand(id, quantity));
        return Ok(product);
    }

    [HttpPost("bulk-increase")]
    public async Task<IActionResult> IncreaseProducts(BulkIncreaseProductsCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("bulk-decrease")]
    public async Task<IActionResult> DecreaseProducts(BulkDecreaseProductsCommand command)
    {
        var result = await Mediator.Send(command);
        var response = Response<bool>.Success(result, 200);
        return Ok(response);
    }
    [HttpGet("stock-history")]
    public async Task<IActionResult> GetStockHistory()
    {
        var stockHistory = await _stockHistoryService.GetStockHistoriesByCompanyAsync();
        var response = Response<List<StockHistoryDto>>.Success(stockHistory, 200);
        return Ok(response);
    }

    [HttpGet("{id}/stock-history")]
    public async Task<IActionResult> GetStockHistoryByProduct(string id)
    {
        var stockHistory = await _stockHistoryService.GetStockHistoriesByProductAsync(id);
        var response = Response<List<StockHistoryDto>>.Success(stockHistory, 200);
        return Ok(response);
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
                MinRequire: product.MinRequire,
                ImageUrl: product.ImageUrl
            ));
        }
        var response = Response<NoContent>.Success(200);
        return Ok(response);
    }

    [HttpPost("export-data")]
    //public async Task<IActionResult> ExportProducts(DateTimePeriod period)
    public async Task<IActionResult> ExportProducts()
    {
        var products = await Mediator.Send(new GetProducts());
        var detailedProducts = products.Select(x => new ExportProductDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            PurchasePrice = x.PurchasePrice,
            SellPrice = x.SellPrice,
            Quantity = x.Quantity,
            //Ordered = x.OrderItems.Sum(x=>x.Quantity)
        }).ToList();
        var response = Response<List<ExportProductDto>>.Success(detailedProducts, 200);
        return Ok(response);
    }
}
