using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using VecoBackend.Data;
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
                    description = task.description,
                    type = task.type,
                    isSeen = task.isSeen
                }).ToListAsync();
            var answer = new List<TaskModel>();
            tasks.ForEach(task =>
            {
                answer.Add(new TaskModel
                {
                    id = task.id,
                    points = task.points,
                    description = task.description,
                    type = task.type,
                    isSeen = task.isSeen
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
    public async Task<List<TaskModel>> GetTasks(string token,TaskStatus status)
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
                    description = task.description,
                    type = task.type,
                    isSeen = task.isSeen
                }).ToListAsync();
            var answer = new List<TaskModel>();
            tasks.ForEach(task =>
            {
                answer.Add(new TaskModel
                {
                    id = task.id,
                    points = task.points,
                    description = task.description,
                    type = task.type,
                    isSeen = task.isSeen
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

    public async Task<int> ChangeTaskStatus(string token, TaskStatus newStatus, int taskId)
    {
        try
        {
            var user = await _context.UserModels.FirstOrDefaultAsync(u => u.token == token);
            if (user == null) return -1;
            var userTask =
                await _context.UserTaskModels.FirstOrDefaultAsync(ut => ut.user_id == user.id && ut.task_id == taskId);
            if (userTask == null) return -1;
            userTask.task_status = newStatus;
            await _context.SaveChangesAsync();
            return userTask.id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return -1;
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
                    description = task.description,
                    type = task.type,
                    name = user.name
                }).ToListAsync();
            var answer = new List<CheckTaskListResponse>();
            foreach (var task in tasks)
            {
                answer.Add(new CheckTaskListResponse()
                {
                    userTaskId = task.id,
                    points = task.points,
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