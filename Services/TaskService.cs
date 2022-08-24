using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using VecoBackend.Data;
using VecoBackend.Models;


namespace VecoBackend.Services;

public class TaskService
{
    private readonly string _connectionString;
    private  ApplicationContext context;

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
                var tasks = await context.TaskModels.Where(s=> s.id == 1).ToListAsync();
                return tasks;
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}