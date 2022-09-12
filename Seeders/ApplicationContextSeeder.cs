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

    public void Seed(string SecretKey, string Issuer, string Audience)
    {
        _applicationContext.TaskModels.RemoveRange(_applicationContext.TaskModels);
        _applicationContext.UserModels.RemoveRange(_applicationContext.UserModels);
        _applicationContext.UserTaskModels.RemoveRange(_applicationContext.UserTaskModels);
        _applicationContext.TaskImageModels.RemoveRange(_applicationContext.TaskImageModels);
        _applicationContext.ImageStorageModels.RemoveRange(_applicationContext.ImageStorageModels);
        _applicationContext.MaterialModels.RemoveRange(_applicationContext.MaterialModels);
        _applicationContext.MaterialImageModels.RemoveRange(_applicationContext.MaterialImageModels);
        _applicationContext.SaveChanges();

        var materialImages = new List<MaterialImageModel>()
        {
            new MaterialImageModel()
            {
                Id = 1,
                MaterialId = 1,
                ImageId = 4,
            },
        };
        _applicationContext.MaterialImageModels.AddRange(materialImages);

        var material = new List<MaterialModel>()
        {
            new MaterialModel()
            {
                Id = 1,
                Title = "Жизнь IT-инженера в Эстонии: тотальная русскоязычность, прекрасная экология и низкие зарплаты",
                Description = Strings.FirstSeederString,
                Author = "Иван Иванов",
                Date = new DateTime(2022, 8, 5),
                IsSeen = true,
                Category = MaterialCategory.Default,
            }
        };
        _applicationContext.MaterialModels.AddRange(material);

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
                imagePath = "boxes/ArcoLinux_2022-03-10_14-25-27.png",
                isUsed = true,
                imageType = ImageType.Task,
            },
            new ImageStorageModel()
            {
                id = 2,
                userId = 3,
                imagePath = "boxes/ArcoLinux_2022-03-10_14-25-27.png",
                isUsed = true,
                imageType = ImageType.Task,
            },
            new ImageStorageModel()
            {
                id = 3,
                userId = 3,
                imagePath = "boxes/ArcoLinux_2022-03-10_14-25-27.png",
                isUsed = true,
                imageType = ImageType.Task,
            },
            new ImageStorageModel()
            {
                id = 4,
                userId = 3,
                imagePath = "boxes/image 1.png",
                isUsed = true,
                imageType = ImageType.Material,
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
                Description =
                    "При походе в магазин вы не должны использовать пластиковые пакеты. Рекомендуем взять с собой шоппер/рюкзак/авоську.",
                Type = TaskType.Day,
                Deadline = DateTime.Today.AddDays(1),
                IsSeen = true
            });
            tasks.Add(new TaskModel()
            {
                Id = 2,
                Points = 800,
                Title = "Сдать свой недельный мусор в пункт переработки",
                Description = "собрать мусор за неделю и сдать в пункт переработки",
                Type = TaskType.Week,
                Deadline = DateTime.Today.AddDays(7),
                IsSeen = true
            });
            tasks.Add(new TaskModel()
            {
                Id = 3,
                Points = 2500,
                Title = "Провести уборку в парке своего района",
                Description =
                    "Огромное количество людей летом проводят свой отдых в парке, часто забывая про санитарные правила, станьте волонтером и очистите свой парк от мусора",
                Type = TaskType.Month,
                Deadline = DateTime.Today.AddMonths(1),
                IsSeen = true
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

        var users_tasks = new List<UserTaskModel>()
        {
            new UserTaskModel()
            {
                Id = 9,
                UserId = 3,
                TaskId = 1,
                taskStatus = TaskStatus.OnCheck,
            },
        };
        _applicationContext.UserTaskModels.AddRange(users_tasks);
        

        _applicationContext.SaveChanges();
    }
}