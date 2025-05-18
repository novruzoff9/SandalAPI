using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Subscription.Application.DTOs.CompanySubscription;
using Subscription.Application.Features.CompanyiesSubscriptions.Commands;
using Subscription.Application.Features.CompanyiesSubscriptions.Queries;
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
        return Ok(companySubscription);
    }

    [HttpGet("company/my")]
    public async Task<IActionResult> GetMySubscription()
    {
        var mySubscriptions = await Mediator.Send(new GetMyCompanySubscriptionQuery());
        var response = Response<CompanySubscriptionDto>.Success(mySubscriptions, 200);
        return Ok(mySubscriptions);
    }
    [HttpPost]
    public async Task<IActionResult> Create(AssignSubscriptionToCompanyCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
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
