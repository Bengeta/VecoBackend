using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using VecoBackend.Data;
using VecoBackend.Enums;
using VecoBackend.Models;
using VecoBackend.Responses;
using TaskStatus = VecoBackend.Enums.TaskStatus;


namespace VecoBackend.Services;

public class TaskService
{
    private ApplicationContext _context;

    public void AddContext(ApplicationContext applicationContext)
    {
        _context = applicationContext;
    }

    public async Task<List<TaskModel>> GetAllTasks(string token)
    {
        try
        {
            var tasks = await (from user in _context.UserModels
                join userTask in _context.UserTaskModels on user.id equals userTask.UserId
                join task in _context.TaskModels on userTask.TaskId equals task.Id
                where user.token == token
                select new
                {
                    id = task.Id,
                    points = task.Points,
                    title = task.Title,
                    description = task.Description,
                    type = task.Type,
                    isSeen = task.IsSeen,
                    deadline = task.Deadline,
                    status = userTask.taskStatus
                }).ToListAsync();
            var answer = new List<TaskModel>();
            tasks.ForEach(task =>
            {
                answer.Add(new TaskModel
                {
                    Id = task.id,
                    Points = task.points,
                    Title = task.title,
                    Description = task.description,
                    Type = task.type,
                    IsSeen = task.isSeen,
                    Deadline = task.deadline,
                    Status = task.status
                });
            });

            return answer;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<TaskModel>> GetTasks(string token, TaskStatus status)
    {
        try
        {
            var tasks = await (from user in _context.UserModels
                join userTask in _context.UserTaskModels on user.id equals userTask.UserId
                join task in _context.TaskModels on userTask.TaskId equals task.Id
                where user.token == token && userTask.taskStatus == status
                select new
                {
                    id = task.Id,
                    points = task.Points,
                    title = task.Title,
                    description = task.Description,
                    type = task.Type,
                    isSeen = task.IsSeen,
                    deadline = task.Deadline,
                }).ToListAsync();
            var answer = new List<TaskModel>();
            tasks.ForEach(task =>
            {
                answer.Add(new TaskModel
                {
                    Id = task.id,
                    Points = task.points,
                    Title = task.title,
                    Description = task.description,
                    Type = task.type,
                    IsSeen = task.isSeen,
                    Deadline = task.deadline,
                });
            });

            return answer;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResponseModel<int>> ChangeTaskStatus(string token, TaskStatus newStatus, int taskId)
    {
        try
        {
            var UserTask =
                await (from user in _context.UserModels
                    join userTask in _context.UserTaskModels on user.id equals userTask.UserId
                    join task in _context.TaskModels on userTask.TaskId equals task.Id
                    where user.token == token && task.Id == taskId
                    select userTask).FirstOrDefaultAsync();
            if (UserTask == null)
                return new ResponseModel<int>() {ResultCode = ResultCode.TaskNotFound};
            UserTask.taskStatus = newStatus;
            await _context.SaveChangesAsync();
            return new ResponseModel<int>() {ResultCode = ResultCode.Success};
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ResponseModel<int>() {ResultCode = ResultCode.Failed};
        }
    }

    public async Task<List<CheckTaskListResponse>> GetCheckTaskList()
    {
        try
        {
            var tasks = await (from user in _context.UserModels
                join userTask in _context.UserTaskModels on user.id equals userTask.UserId
                join task in _context.TaskModels on userTask.TaskId equals task.Id
                where userTask.taskStatus == TaskStatus.OnCheck
                select new
                {
                    id = userTask.Id,
                    points = task.Points,
                    title = task.Title,
                    description = task.Description,
                    type = task.Type,
                    name = user.name,
                }).ToListAsync();
            var answer = new List<CheckTaskListResponse>();
            foreach (var task in tasks)
            {
                answer.Add(new CheckTaskListResponse()
                {
                    userTaskId = task.id,
                    points = task.points,
                    title = task.title,
                    description = task.description,
                    UserName = task.name,
                });
            }

            return answer;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<TaskModel> GetTaskById(int id)
    {
        try
        {
            var task = await _context.TaskModels.Where(u => u.Id == id).FirstOrDefaultAsync();
            return task;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
    public async Task AddTask(AddTaskResponse task)
    {
        try
        {
            var newTask = new TaskModel()
            {
                Title = task.Title,
                Description = task.Description,
                Type = task.Type,
                Points = task.Points,
                Deadline = task.Deadline,
                IsSeen = false,
            };
            await _context.TaskModels.AddAsync(newTask);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task ChangeTaskVisibility(int taskId, bool visibility)
    {
        try
        {
            var task = await _context.TaskModels.Where(u => u.Id == taskId).FirstOrDefaultAsync();
            task.IsSeen = visibility;
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task DeleteTask(int taskId)
    {
        try
        {
            var task = await _context.TaskModels.Where(u => u.Id == taskId).FirstOrDefaultAsync();
            _context.TaskModels.Remove(task);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task ChangeTask(ChangeTaskResponse task)
    {
        try
        {
            var newTask = await _context.TaskModels.Where(u => u.Id == task.Id).FirstOrDefaultAsync();
            newTask.Title = task.Title;
            newTask.Description = task.Description;
            newTask.Type = task.Type;
            newTask.Points = task.Points;
            newTask.Deadline = task.Deadline;
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task<List<TaskModel>> GetTasksByType(TaskType type)
    {
        try
        {
            var tasks = await _context.TaskModels.Where(u => u.Type == type).ToListAsync();
            return tasks;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    
}