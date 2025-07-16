using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Export;
using Shared.DTOs.General;
using Shared.ResultTypes;
using System.Linq.Expressions;
using System.Text.Json;
using Template.WebAPI.DTOs.Response;
using Template.WebAPI.Enums;
using Template.WebAPI.Services;

namespace Template.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TemplatesController : ControllerBase
{
    private readonly ITemplateGenerator _templateGenerator;
    private readonly HttpClient _localClient;

    public TemplatesController(ITemplateGenerator templateGenerator, IHttpClientFactory httpClientFactory)
    {
        _templateGenerator = templateGenerator;
        _localClient = httpClientFactory.CreateClient("LocalClient");
    }

    [HttpGet("import/{type}")]
    public IActionResult DownloadExcelImportTemplate(string type)
    {
        if (!Enum.TryParse(type, true, out ImportType importType))
            return BadRequest("Invalid import type");
        try
        {
            var fileBytes = _templateGenerator.GenerateExcelImportTemplate(importType);
            var fileName = $"{importType}_template.xlsx";

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("export/{type}")]
    public async Task<IActionResult> DownloadExcelExportTemplate(string type, 
        [FromBody] DateTimePeriod period = null)
    {
        if (!Enum.TryParse(type, true, out ExportType exportType))
            return BadRequest("Invalid export type");

        var content = JsonContent.Create(period);
        var data = await _localClient.PostAsync($"/{exportType}/export-data", content);
        string responseData = await data.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        if (!data.IsSuccessStatusCode)
        {
            var errorResponse = JsonSerializer.Deserialize<ResponseDto<string>>(responseData, options);
            return StatusCode(errorResponse!.StatusCode, errorResponse);
        }
        var dtoType = exportType switch
        {
            ExportType.Product => typeof(ResponseDto<List<ExportProductDto>>),

            ExportType.Customer => typeof(ResponseDto<List<ExportCustomerDto>>),

            ExportType.Warehouse => typeof(ResponseDto<List<ExportWarehouseDto>>),

            ExportType.Order => typeof(ResponseDto<List<ExportOrderDto>>),

            _ => typeof(ResponseDto<List<ExportCustomerDto>>)
        };

        dynamic response = JsonSerializer.Deserialize(responseData, dtoType, options);
        if (response == null || response.Data == null)
            throw new Exception("No data available for export");
        var fileBytes = _templateGenerator.GenerateExcelExportTemplate(response.Data);
        var fileName = $"{nameof(response.Data)}_export.xlsx";
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}