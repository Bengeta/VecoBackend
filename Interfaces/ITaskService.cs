using VecoBackend.Enums;
using VecoBackend.Models;
using VecoBackend.Responses;
using TaskStatus = VecoBackend.Enums.TaskStatus;

namespace VecoBackend.Interfaces;

public interface ITaskService
{
    public Task<List<GetTaskResponse>> GetAllTasks(string token);
    public Task<List<GetTaskResponse>> GetTasks(string token, TaskStatus status);
    public Task<ResponseModel<int>> ChangeTaskStatus(string token, TaskStatus newStatus, int taskId);
    public Task<List<CheckTaskListResponse>> GetCheckTaskList();
    public Task<GetTaskResponse> GetTaskById(int id);
    public Task AddTask(AddTaskResponse task);
    public Task ChangeTaskVisibility(int taskId, bool visibility);
    public Task DeleteTask(int taskId);
    public Task ChangeTask(ChangeTaskResponse task);
    public  Task<List<TaskModel>> GetTasksByType(TaskType type);
    public  Task<ResultCode> SubmitImages(List<int> imagesId, int taskId, string token);
    
}