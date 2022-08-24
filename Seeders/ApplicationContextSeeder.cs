using VecoBackend.Data;
using VecoBackend.Interfaces;
using VecoBackend.Models;
using TaskStatus = VecoBackend.Interfaces.TaskStatus;

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
        var tasks = new List<TaskModel>();
        if (!_applicationContext.TaskModels.Any())
        {
            tasks.Add(new TaskModel()
            {
                id = 1,
                points = 200,
                description = "Сходить в магазин с собственной сумкой",
                type = TaskType.Day,
            });
            tasks.Add(new TaskModel()
            {
                id = 2,
                points = 800,
                description = "Сдать свой недельный мусор в пункт переработки",
                type = TaskType.Week
            });
            tasks.Add(new TaskModel()
            {
                id = 3,
                points = 2500,
                description = "Провести уборку в парке своего района",
                type = TaskType.Month
            });
            _applicationContext.TaskModels.AddRange(tasks);
        }

        var users = new List<UserModel>();
        if (!_applicationContext.UserModels.Any())
        {
            users.Add(new UserModel()
            {
                id = 1,
                name = "Иван Иванов",
                isAdmin = false,
                token = "asdf"
            });
            users.Add(new UserModel()
            {
                id = 2,
                name = "Илья Ильин",
                isAdmin = false,
                token = "asdf"
            });
            users.Add(new UserModel()
            {
                id = 3,
                name = "Руслан Гасанов",
                isAdmin = true,
                token = "asdf"
            });
            _applicationContext.UserModels.AddRange(users);
        }

        if (!_applicationContext.UserTaskModels.Any())
        {
            var users_tasks = new List<UserTaskModel>();
            users.ForEach(u =>
                tasks.ForEach(task => users_tasks.Add(new UserTaskModel()
                {
                    user_id = u.id,
                    task_id = task.id,
                    task_status = TaskStatus.Created,
                    photos = "kj"
                }))
            );
            _applicationContext.UserTaskModels.AddRange(users_tasks);
        }

        _applicationContext.SaveChanges();
    }
}