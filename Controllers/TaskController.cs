using a;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using VecoBackend.Data;
using VecoBackend.Responses;
using VecoBackend.Services;
using TaskStatus = VecoBackend.Enums.TaskStatus;

namespace VecoBackend.Controllers;
[ApiController]
[Authorize]
public class TaskController : ControllerBase
{
    private TaskService _taskService;

    public TaskController(TaskService taskService,ApplicationContext _applicationContext)
    {
        _taskService = taskService;
        taskService.AddContext(_applicationContext);
    }

    [HttpGet]
    [Route("tasks/all")]
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

    [HttpGet]
    [Route("tasks/uncompleted")]
    public async Task<IActionResult> GetUserUncompletedTasks()
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var tasks = await _taskService.GetTasks(token,TaskStatus.Created);
        if(tasks == null)
        {
            return BadRequest();
        }
        var ans = new TaskListResponse();
        ans.tasks = tasks;
        return Ok(ans);
    }
    [HttpGet]
    [Route("tasks/onprogress")]
    public async Task<IActionResult> GetUserOnProgressTasks()
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var tasks = await _taskService.GetTasks(token,TaskStatus.OnCheck);
        if(tasks == null)
        {
            return BadRequest();
        }
        var ans = new TaskListResponse();
        ans.tasks = tasks;
        return Ok(ans);
    }
    [HttpGet]
    [Route("tasks/completed")]
    public async Task<IActionResult> GetUserCompletedTasks()
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var tasks = await _taskService.GetTasks(token,TaskStatus.Finished);
        if(tasks == null)
        {
            return BadRequest();
        }
        var ans = new TaskListResponse();
        ans.tasks = tasks;
        return Ok(ans);
    }
    
    [HttpGet]
    [Route("tasks/{id}")]
    public async Task<IActionResult> GetUserTaskById(int id)
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var task = await _taskService.GetTaskById(id);
        if(task == null)
        {
            return BadRequest();
        }
        return Ok(task);
    }
    
    [HttpPut]
    [Route("status")]
    public async Task<IActionResult> ChangeTaskStatus(ChangeTaskStatusRequest request)
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var tasksId = await _taskService.ChangeTaskStatus(token, request.newStatus, request.taskId);
        if(tasksId == -1)
        {
            return BadRequest();
        }
        return Ok(tasksId);
    }
}