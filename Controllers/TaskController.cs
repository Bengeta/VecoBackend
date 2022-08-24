using a;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


    [HttpPost]
    [Route("task/all")]
    public async Task<IActionResult> GetUserAllTasks(TokenRequest tokenResponse)
    {
        var tasks = await _taskService.GetAllTasks(tokenResponse.token);
        if(tasks == null)
        {
            return BadRequest();
        }
        var ans = new TaskListResponse();
        ans.tasks = tasks;
        return Ok(ans);
    }
}