using VecoBackend.Data;
using VecoBackend.Interfaces;
using VecoBackend.Models;

namespace VecoBackend.Seeders;

public class ApplicationContextSeeder
{
    private readonly ApplicationContext _applicationContext;

    public ApplicationContextSeeder(ApplicationContext _applicationContext)
    {
        this._applicationContext = _applicationContext;
    }

    public void Seed()
    {
        if (!_applicationContext.TaskModels.Any())
        {
            var tasks = new List<TaskModel>()
            {
                new TaskModel()
                {
                    id = 1,
                    points = 200,
                    description = "Сходить в магазин с собственной сумкой",
                    type = TaskType.Day,
                },
                new TaskModel()
                {
                    id = 2,
                    points = 800,
                    description = "Сдать свой недельный мусор в пункт переработки",
                    type = TaskType.Week
                },
                new TaskModel()
                {
                    id = 3,
                    points = 2500,
                    description = "Провести уборку в парке своего района",
                    type = TaskType.Month
                },
            };

            _applicationContext.TaskModels.AddRange(tasks);
        }

        if (!_applicationContext.UserModels.Any())
        {
            var users = new List<UserModel>()
            {
                new UserModel()
                {
                    id = 1,
                    name = "Иван Иванов",
                    isAdmin = false,
                },
                new UserModel()
                {
                    id = 2,
                    name = "Илья Ильин",
                    isAdmin = false,
                },
                new UserModel()
                {
                    id = 3,
                    name = "Руслан Гасанов",
                    isAdmin = true,
                },
            };

            _applicationContext.UserModels.AddRange(users);
        }

        _applicationContext.SaveChanges();
    }
}