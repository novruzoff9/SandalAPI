using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.ResultTypes;
using Subscription.Application.SubscriptionPackages;
using Subscription.Domain.Entities;

namespace Subscription.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubscriptionController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var packages = await Mediator.Send(new GetSubscriptionPackagesQuery());
        var response = Response<List<SubscriptionPackage>>.Success(packages, 200);
        return Ok(response);
    }
}
