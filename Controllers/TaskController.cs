using a;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using VecoBackend.Data;
using VecoBackend.Services;

namespace VecoBackend.Controllers;

public class TaskController : ControllerBase
{
    private TaskService _taskService;

    public TaskController(TaskService taskService,ApplicationContext _applicationContext)
    {
        _taskService = taskService;
        taskService.AddContext(_applicationContext);
    }


    [HttpGet]
    [Route("task/all")]
    public async Task<IActionResult> GetUserAllTasks()
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var tasks = await _taskService.GetAllTasks(token);
        if(tasks == null)
        {
            return BadRequest();
        }
        var ans = new TaskListResponse();
        ans.tasks = tasks;
        return Ok(ans);
    }
}