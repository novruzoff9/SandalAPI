using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Organization.Application.Companies.Commands.CreateCompanyCommand;
using Organization.Application.Companies.Commands.DeleteCompanyCommand;
using Organization.Application.Companies.Commands.EditCompanyCommand;
using Organization.Application.Companies.Queries.GetCompaniesQuery;
using Organization.Application.Companies.Queries.GetCompanyQuery;
using Organization.Application.DTOs.Company;
using Shared.ResultTypes;

namespace Organization.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class BaseController : ControllerBase
{
    private IMediator _mediator;
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
}

[Authorize(Roles="admin")]
public class CompanyController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var companies = await Mediator.Send(new GetCompanies());
        List<CompanyDto> companiesDto = new();
        foreach (var company in companies)
        {
            CompanyDto companyDto = new()
            {
                Id = company.Id,
                Name = company.Name,
                Warehouses = company.Warehouses.Count
            };
            companiesDto.Add(companyDto);
        }
        var response = Response<List<CompanyDto>>.Success(companiesDto, 200);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var response = await Mediator.Send(new GetCompany(id));
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCompany command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, EditCompany command)
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
        var response = await Mediator.Send(new DeleteCompany(id));
        return Ok(response);
    }
}
