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
    public Task<bool> CreateTask(AddTaskResponse task);
    public Task ChangeTaskVisibility(int taskId, bool visibility);
    public Task<bool> DeleteTask(int taskId);
    public Task<bool> UpdateTask(ChangeTaskResponse task);
    public  Task<List<TaskModel>> GetTasksByType(TaskType type);
    public  Task<ResultCode> SubmitImages(List<int> imagesId, int taskId, string token);
    public Task<PagedList<TaskModel>> GetTasks(int page, int pageSize);

}