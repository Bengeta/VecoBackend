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
                join userTask in _context.UserTaskModels on user.id equals userTask.user_id
                join task in _context.TaskModels on userTask.task_id equals task.id
                where user.token == token
                select new
                {
                    id = task.id,
                    points = task.points,
                    title = task.title,
                    description = task.description,
                    type = task.type,
                    isSeen = task.isSeen,
                    deadline = task.deadline,
                    status = userTask.task_status
                }).ToListAsync();
            var answer = new List<TaskModel>();
            tasks.ForEach(task =>
            {
                answer.Add(new TaskModel
                {
                    id = task.id,
                    points = task.points,
                    title = task.title,
                    description = task.description,
                    type = task.type,
                    isSeen = task.isSeen,
                    deadline = task.deadline,
                    status = task.status
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
                join userTask in _context.UserTaskModels on user.id equals userTask.user_id
                join task in _context.TaskModels on userTask.task_id equals task.id
                where user.token == token && userTask.task_status == status
                select new
                {
                    id = task.id,
                    points = task.points,
                    title = task.title,
                    description = task.description,
                    type = task.type,
                    isSeen = task.isSeen,
                    deadline = task.deadline,
                }).ToListAsync();
            var answer = new List<TaskModel>();
            tasks.ForEach(task =>
            {
                answer.Add(new TaskModel
                {
                    id = task.id,
                    points = task.points,
                    title = task.title,
                    description = task.description,
                    type = task.type,
                    isSeen = task.isSeen,
                    deadline = task.deadline,
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
                    join userTask in _context.UserTaskModels on user.id equals userTask.user_id
                    join task in _context.TaskModels on userTask.task_id equals task.id
                    where user.token == token && task.id == taskId
                    select userTask).FirstOrDefaultAsync();
            if (UserTask == null)
                return new ResponseModel<int>() {ResultCode = ResultCode.TaskNotFound};
            UserTask.task_status = newStatus;
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
                join userTask in _context.UserTaskModels on user.id equals userTask.user_id
                join task in _context.TaskModels on userTask.task_id equals task.id
                where userTask.task_status == TaskStatus.OnCheck
                select new
                {
                    id = userTask.id,
                    points = task.points,
                    title = task.title,
                    description = task.description,
                    type = task.type,
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
            var task = await _context.TaskModels.Where(u => u.id == id).FirstOrDefaultAsync();
            return task;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}