using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using VecoBackend.Data;
using VecoBackend.Enums;
using VecoBackend.Models;
using VecoBackend.Utils;
using TaskStatus = VecoBackend.Enums.TaskStatus;

namespace VecoBackend.Seeders;

public class ApplicationContextSeeder
{
    private readonly ApplicationContext _applicationContext;
    public ApplicationContextSeeder(ApplicationContext _applicationContext)
    {
        this._applicationContext = _applicationContext;
    }
    public void Seed(string SecretKey,string Issuer,string Audience)
    {
        _applicationContext.TaskModels.RemoveRange(_applicationContext.TaskModels);
        _applicationContext.UserModels.RemoveRange(_applicationContext.UserModels);
        _applicationContext.UserTaskModels.RemoveRange(_applicationContext.UserTaskModels);
        _applicationContext.TaskImageModels.RemoveRange(_applicationContext.TaskImageModels);
        _applicationContext.ImageStorageModels.RemoveRange(_applicationContext.ImageStorageModels);
        _applicationContext.SaveChanges();

        var photosTask = new List<TaskImageModel>()
        {
            new TaskImageModel()
            {
                id = 1,
                UserTaskId = 9,
                imageId = 1
            },
            new TaskImageModel()
            {
                id = 2,
                UserTaskId = 9,
                imageId = 2
            },
            new TaskImageModel()
            {
                id = 3,
                UserTaskId = 9,
                imageId = 3
            },
        };
        _applicationContext.TaskImageModels.AddRange(photosTask);
        var photos = new List<ImageStorageModel>()
        {
            new ImageStorageModel()
            {
                id = 1,
                userId = 3,
                imagePath = "res/ArcoLinux_2022-03-10_14-25-27.png",
            },
            new ImageStorageModel()
            {
                id = 2,
                userId = 3,
                imagePath = "res/ArcoLinux_2022-03-10_14-25-27.png",
            },
            new ImageStorageModel()
            {
                id = 3,
                userId = 3,
                imagePath = "res/ArcoLinux_2022-03-10_14-25-27.png",
            },
        };
        _applicationContext.ImageStorageModels.AddRange(photos);

        var tasks = new List<TaskModel>();
        if (!_applicationContext.TaskModels.Any())
        {
            tasks.Add(new TaskModel()
            {
                Id = 1,
                Points = 200,
                Title = "Сходить в магазин с собственной сумкой",
                Description = "При походе в магазин вы не должны использовать пластиковые пакеты. Рекомендуем взять с собой шоппер/рюкзак/авоську.",
                Type = TaskType.Day,
                Deadline = DateTime.Today.AddDays(1)
            });
            tasks.Add(new TaskModel()
            {
                Id = 2,
                Points = 800,
                Title = "Сдать свой недельный мусор в пункт переработки",
                Description = "собрать мусор за неделю и сдать в пункт переработки",
                Type = TaskType.Week,
                Deadline = DateTime.Today.AddDays(7)
            });
            tasks.Add(new TaskModel()
            {
                Id = 3,
                Points = 2500,
                Title = "Провести уборку в парке своего района",
                Description = "Огромное количество людей летом проводят свой отдых в парке, часто забывая про санитарные правила, станьте волонтером и очистите свой парк от мусора",
                Type = TaskType.Month,
                Deadline = DateTime.Today.AddMonths(1),
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
                token = "asdw",
                password = "asdf",
                salt = "asdf",
                email = "assdfd@"
            });
            users.Add(new UserModel()
            {
                id = 2,
                name = "Илья Ильин",
                isAdmin = true,
                token = "asdf",
                password = "asd",
                salt = "asdf",
                email = "asd@"
            });
            var pas = Hash.GenerateHash("asdf");
            users.Add(new UserModel()
            {
                id = 3,
                name = "Руслан Гасанов",
                isAdmin = true,
                token = "asdf",
                password = pas.Key,
                salt = pas.Value,
                email = "gsanov@"
            });
            
             List<Claim> claims = new List<Claim>();
             claims.Add(new Claim(ClaimTypes.Name, users[2].name));
             claims.Add(new Claim(ClaimTypes.Email, users[2].email));
            
             var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            
             var token = new JwtSecurityToken(
                 issuer: Issuer,
                audience: Audience,
                claims: claims,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
             );
             users[2].token = new JwtSecurityTokenHandler().WriteToken(token);
            _applicationContext.UserModels.AddRange(users);
        }

        var i = 0;
        if (!_applicationContext.UserTaskModels.Any())
        {
            var users_tasks = new List<UserTaskModel>();
            users.ForEach(u =>
                tasks.ForEach(task => users_tasks.Add(new UserTaskModel()
                {
                    UserId = u.id,
                    TaskId = task.Id,
                    taskStatus = TaskStatus.Created,
                    Id = ++i
                }))
            );
            users_tasks[^1].taskStatus = TaskStatus.OnCheck;
            users_tasks[^2].taskStatus = TaskStatus.Finished;
            _applicationContext.UserTaskModels.AddRange(users_tasks);
        }

        _applicationContext.SaveChanges();
    }
}