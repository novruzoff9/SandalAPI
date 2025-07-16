using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Subscription.Application.DTOs.CompanySubscription;
using Subscription.Application.Features.CompaniesSubscriptions.Commands;
using Subscription.Application.Features.CompaniesSubscriptions.Queries;
using Shared.ResultTypes;

namespace Subscription.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class CompanySubscriptionController : BaseController
{
    [HttpGet("company/{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var companySubscription = await Mediator.Send(new GetCompanyActiveSubscriptionQuery(id));
        var response = Response<CompanySubscriptionDto>.Success(companySubscription, 200);
        return Ok(response);
    }

    [HttpGet("my/active")]
    public async Task<IActionResult> GetMyCompanyActiveSubscription()
    {
        var mySubscriptions = await Mediator.Send(new GetMyCompanyActiveSubscriptionQuery());
        var response = Response<CompanySubscriptionDto>.Success(mySubscriptions, 200);
        return Ok(response);
    }

    [HttpGet("my/history")]
    public async Task<IActionResult> GetMyCompanySubscriptionHistory()
    {
        var companySubscriptions = await Mediator.Send(new GetMyCompanySubscriptionHistoryQuery());
        var response = Response<List<CompanySubscriptionHistoryDto>>.Success(companySubscriptions, 200);
        return Ok(response);
    }
    [HttpPost]
    public async Task<IActionResult> Assign(AssignSubscriptionToCompanyCommand command)
    {
        var result = await Mediator.Send(command);
        var response = Response<string>.Success(result, 201);
        return Ok(response);
    }

    //[HttpGet("company")]
    //public async Task<IActionResult> GetByCompany()
    //{
    //    var companySubscriptions = await Mediator.Send(new GetCompanySubscriptionsQuery());
    //    return Ok(companySubscriptions);
    //}
    //[HttpPut("{id}")]
    //public async Task<IActionResult> Update(string id, EditCompanySubscription command)
    //{
    //    if (id != command.Id)
    //    {
    //        return BadRequest();
    //    }
    //    var result = await Mediator.Send(command);
    //    if (result == false)
    //    {
    //        return NotFound();
    //    }
    //    return Ok(result);
    //}
    //[HttpDelete("{id}")]
    //public async Task<IActionResult> Delete(string id)
    //{
    //    var result = await Mediator.Send(new DeleteCompanySubscriptionCommand(id));
    //    if (result == false)
    //    {
    //        return NotFound();
    //    }
    //    return NoContent();
    //}
}
