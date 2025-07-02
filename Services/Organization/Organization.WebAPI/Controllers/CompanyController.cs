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
using Shared.Services;

namespace Organization.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles="admin")]
public class CompanyController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var companies = await Mediator.Send(new GetCompanies());
        var response = Response<List<CompanyDto>>.Success(companies, 200);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var company = await Mediator.Send(new GetCompany(id));
        var response = Response<CompanyDto>.Success(company, 200);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCompany command)
    {
        var result = await Mediator.Send(command);
        var response = Response<bool>.Success(result, 201);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, EditCompany command)
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
        var result = await Mediator.Send(new DeleteCompany(id));
        if (result == false)
        {
            return NotFound();
        }
        var response = Response<bool>.Success(result, 204);
        return Ok(response);
    }
}
