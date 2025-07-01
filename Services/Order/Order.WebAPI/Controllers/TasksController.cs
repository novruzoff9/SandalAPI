using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.Tasks.Commands.CompleteTaskCommand;
using Order.Application.Features.Tasks.Commands.StartTaskCommand;
using Order.Application.Features.Tasks.Queries;
using System.Net;

namespace Order.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "warehouseman")]
public class TasksController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetTasks()
    {
        var tasks = await Mediator.Send(new GetTasksQuery());
        var response = Response<List<OrderShowDto>>.Success(tasks, 200);
        return Ok(response);
    }

    [HttpGet("{id}")]
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
    [HttpGet("completed")]
    public async Task<IActionResult> GetCompletedTasks()
    {
        var tasks = await Mediator.Send(new GetCompletedTasksQuery());
        var response = Response<List<OrderShowDto>>.Success(tasks, 200);
        return Ok(response);
    }

    [HttpPut("{id}/Start")]
    public async Task<IActionResult> StartTask(string id)
    {
        var result = await Mediator.Send(new StartTaskCommand(id));
        if (!result)
        {
            return BadRequest(Response<NoContent>.Fail("Failed to start task", 400));
        }
        return Ok(Response<NoContent>.Success(204));
    }

    [HttpPut("{id}/Complete")]
    public async Task<IActionResult> CompleteTask(string id, CompleteOrderRequest request)
    {
        var result = await Mediator.Send(new CompleteTaskCommand(id, request.Products));
        if (!result)
        {
            return BadRequest(Response<NoContent>.Fail("Failed to complete task", 400));
        }
        return Ok(Response<NoContent>.Success(204));
    }
}
