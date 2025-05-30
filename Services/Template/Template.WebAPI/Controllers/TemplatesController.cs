using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Template.WebAPI.Enums;
using Template.WebAPI.Services;

namespace Template.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TemplatesController : ControllerBase
{
    private readonly ITemplateGenerator _templateGenerator;

    public TemplatesController(ITemplateGenerator templateGenerator)
    {
        _templateGenerator = templateGenerator;
    }

    [HttpGet("import/{type}")]
    public IActionResult DownloadExcelTemplate(string type)
    {
        if (!Enum.TryParse(type, true, out ImportType importType))
            return BadRequest("Invalid import type");
        try
        {
            var fileBytes = _templateGenerator.GenerateExcelTemplate(importType);
            var fileName = $"{importType}_template.xlsx";

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
