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
        _applicationContext.TaskPhotoModels.RemoveRange(_applicationContext.TaskPhotoModels);
        _applicationContext.SaveChanges();

        var photos = new List<TaskPhotoModel>()
        {
            new TaskPhotoModel()
            {
                UserTaskId = 7,
                photoPath = "res/ArcoLinux_2022-02-20_15-12-32.png"
            },
            new TaskPhotoModel()
            {
                UserTaskId = 7,
                photoPath = "res/ArcoLinux_2022-03-10_14-25-27.png"
            },
            new TaskPhotoModel()
            {
                UserTaskId = 7,
                photoPath = "res/ArcoLinux_2022-02-20_15-12-32.png"
            },
        };
        _applicationContext.TaskPhotoModels.AddRange(photos);

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
                    user_id = u.id,
                    task_id = task.id,
                    task_status = TaskStatus.Created,
                    id = ++i
                }))
            );
            users_tasks[^1].task_status = TaskStatus.OnCheck;
            users_tasks[^2].task_status = TaskStatus.Finished;
            _applicationContext.UserTaskModels.AddRange(users_tasks);
        }

        _applicationContext.SaveChanges();
    }
}