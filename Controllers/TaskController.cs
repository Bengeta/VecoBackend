using a;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using VecoBackend.Data;
using VecoBackend.Enums;
using VecoBackend.Models;
using VecoBackend.Responses;
using VecoBackend.Services;
using TaskStatus = VecoBackend.Enums.TaskStatus;

namespace VecoBackend.Controllers;

[ApiController]
[Authorize]
public class TaskController : ControllerBase
{
    private TaskService _taskService;

    public TaskController(TaskService taskService, ApplicationContext _applicationContext)
    {
        _taskService = taskService;
        taskService.AddContext(_applicationContext);
    }

    [HttpGet]
    [Route("tasks/all")]
    public async Task<ResponseModel<List<TaskModel>>> GetUserAllTasks()
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var tasks = await _taskService.GetAllTasks(token);
        if (tasks == null)
            return new ResponseModel<List<TaskModel>>() {ResultCode = ResultCode.Failed};
        return new ResponseModel<List<TaskModel>>() {ResultCode = ResultCode.Success, Data = tasks};
    }

    [HttpGet]
    [Route("tasks/uncompleted")]
    public async Task<ResponseModel<List<TaskModel>>> GetUserUncompletedTasks()
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var tasks = await _taskService.GetTasks(token, TaskStatus.Created);
        if (tasks == null)
            return new ResponseModel<List<TaskModel>>() {ResultCode = ResultCode.Failed};
        return new ResponseModel<List<TaskModel>>() {ResultCode = ResultCode.Success, Data = tasks};
    }

    [HttpGet]
    [Route("tasks/onprogress")]
    public async Task<ResponseModel<List<TaskModel>>> GetUserOnProgressTasks()
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var tasks = await _taskService.GetTasks(token, TaskStatus.OnCheck);
        if (tasks == null)
            return new ResponseModel<List<TaskModel>>() {ResultCode = ResultCode.Failed};
        return new ResponseModel<List<TaskModel>>() {ResultCode = ResultCode.Success, Data = tasks};
    }

    [HttpGet]
    [Route("tasks/completed")]
    public async Task<ResponseModel<List<TaskModel>>> GetUserCompletedTasks()
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var tasks = await _taskService.GetTasks(token, TaskStatus.Finished);
        if (tasks == null)
            return new ResponseModel<List<TaskModel>>() {ResultCode = ResultCode.Failed};
        return new ResponseModel<List<TaskModel>>() {ResultCode = ResultCode.Success, Data = tasks};
    }

    [HttpGet]
    [Route("tasks/{id}")]
    public async Task<ResponseModel<TaskModel>> GetUserTaskById(int id)
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var task = await _taskService.GetTaskById(id);
        if (task == null)
            return new ResponseModel<TaskModel>() {ResultCode = ResultCode.Failed};
        return new ResponseModel<TaskModel>() {ResultCode = ResultCode.Success, Data = task};
    }

    [HttpPut]
    [Route("task/status")]
    public async Task<ResponseModel<int>> ChangeTaskStatus(ChangeTaskStatusRequest request)
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        return await _taskService.ChangeTaskStatus(token, request.newStatus, request.taskId);
    }
}