using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.Tasks.Queries;

namespace Order.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehousemanOrdersController : BaseController
{
    [HttpGet("Tasks")]
    public async Task<IActionResult> GetTasks()
    {
        var tasks = await Mediator.Send(new GetTasksQuery());
        var response = Response<List<OrderShowDto>>.Success(tasks, 200);
        return Ok(response);
    }

    [HttpGet("Tasks/{id}")]
    public async Task<IActionResult> GetTaskDetails(string id)
    {
        var taskDetails = await Mediator.Send(new GetTaskDetailsQuery(id));
        if (taskDetails == null)
        {
            return NotFound(Response<NoContent>.Fail("Task not found", 404));
        }
        var response = Response<OrderShowDto>.Success(taskDetails, 200);
        return Ok(response);
    }
}
