using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using VecoBackend.Data;
using VecoBackend.Models;
using TaskStatus = VecoBackend.Enums.TaskStatus;


namespace VecoBackend.Services;

public class TaskService
{
    private readonly string _connectionString;
    private ApplicationContext context;

    public TaskService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MainDB");
        if (_connectionString == null) throw new Exception("Connection string not specified");
    }

    public void AddContext(ApplicationContext _applicationContext)
    {
        context = _applicationContext;
    }

    public async Task<List<TaskModel>> GetAllTasks(string token)
    {
        try
        {
            var tasks = await (from user in context.UserModels
                join user_task in context.UserTaskModels on user.id equals user_task.user_id
                join task in context.TaskModels on user_task.task_id equals task.id
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
    
    public async Task<int> ChangeTaskStatus(string token,TaskStatus newStatus, int taskId)
    {
        try
        {
            var user = await context.UserModels.FirstOrDefaultAsync(u => u.token == token);
            if (user == null) return -1;
            var userTask = await context.UserTaskModels.FirstOrDefaultAsync(ut => ut.user_id == user.id && ut.task_id == taskId);
            if (userTask == null) return -1;
            userTask.task_status = newStatus;
            await context.SaveChangesAsync();
            return userTask.id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return -1;
        }
    }


}