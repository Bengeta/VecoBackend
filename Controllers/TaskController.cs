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
public class TaskController : BaseController
{
    private readonly TaskService _taskService;

    public TaskController(TaskService taskService, ApplicationContext _applicationContext)
    {
        _taskService = taskService;
        taskService.AddContext(_applicationContext);
    }

    [HttpGet]
    [Route("tasks/all")]
    public async Task<ResponseModel<List<GetTaskResponse>>> GetUserAllTasks()
    {
        var token = Token();
        var tasks = await _taskService.GetAllTasks(token);
        if (tasks == null)
            return new ResponseModel<List<GetTaskResponse>>() {ResultCode = ResultCode.Failed};
        return new ResponseModel<List<GetTaskResponse>>() {ResultCode = ResultCode.Success, Data = tasks};
    }

    [HttpGet]
    [Route("tasks/uncompleted")]
    public async Task<ResponseModel<List<GetTaskResponse>>> GetUserUncompletedTasks()
    {
        var token = Token();
        var tasks = await _taskService.GetTasks(token, TaskStatus.Created);
        if (tasks == null)
            return new ResponseModel<List<GetTaskResponse>>() {ResultCode = ResultCode.Failed};
        return new ResponseModel<List<GetTaskResponse>>() {ResultCode = ResultCode.Success, Data = tasks};
    }

    [HttpGet]
    [Route("tasks/onprogress")]
    public async Task<ResponseModel<List<GetTaskResponse>>> GetUserOnProgressTasks()
    {
        var token = Token();
        var tasks = await _taskService.GetTasks(token, TaskStatus.OnCheck);
        if (tasks == null)
            return new ResponseModel<List<GetTaskResponse>>() {ResultCode = ResultCode.Failed};
        return new ResponseModel<List<GetTaskResponse>>() {ResultCode = ResultCode.Success, Data = tasks};
    }

    [HttpGet]
    [Route("tasks/completed")]
    public async Task<ResponseModel<List<GetTaskResponse>>> GetUserCompletedTasks()
    {
        var token = Token();
        var tasks = await _taskService.GetTasks(token, TaskStatus.Finished);
        if (tasks == null)
            return new ResponseModel<List<GetTaskResponse>>() {ResultCode = ResultCode.Failed};
        return new ResponseModel<List<GetTaskResponse>>() {ResultCode = ResultCode.Success, Data = tasks};
    }

    [HttpGet]
    [Route("tasks/{id}")]
    public async Task<ResponseModel<GetTaskResponse>> GetUserTaskById(int id)
    {
        var token = Token();
        var task = await _taskService.GetTaskById(id);
        if (task == null)
            return new ResponseModel<GetTaskResponse>() {ResultCode = ResultCode.Failed};
        return new ResponseModel<GetTaskResponse>() {ResultCode = ResultCode.Success, Data = task};
    }

    [HttpPut]
    [Route("task/status")]
    public async Task<ResponseModel<int>> ChangeTaskStatus(ChangeTaskStatusRequest request)
    {
        var token = Token();
        return await _taskService.ChangeTaskStatus(token, request.newStatus, request.taskId);
    }
}