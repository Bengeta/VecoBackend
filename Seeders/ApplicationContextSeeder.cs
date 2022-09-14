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


        var tasks = new List<TaskModel>();
        if (!_applicationContext.TaskModels.Any())
        {
            tasks.Add(new TaskModel()
            {
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
                Points = 800,
                Title = "Сдать свой недельный мусор в пункт переработки",
                Description = "собрать мусор за неделю и сдать в пункт переработки",
                Type = TaskType.Week,
                Deadline = DateTime.Today.AddDays(7),
                IsSeen = true
            });
            tasks.Add(new TaskModel()
            {
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
                name = "Иван Иванов",
                isAdmin = false,
                token = "asdw",
                password = "asdf",
                salt = "asdf",
                email = "assdfd@"
            });
            users.Add(new UserModel()
            {
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


        var material = new List<MaterialModel>()
        {
            new MaterialModel()
            {
                Title = "Жизнь IT-инженера в Эстонии: тотальная русскоязычность, прекрасная экология и низкие зарплаты",
                Description = Strings.FirstSeederString,
                Author = "Иван Иванов",
                Date = new DateTime(2022, 8, 5),
                IsSeen = true,
                Category = MaterialCategory.Default,
            },
            new MaterialModel()
            {
                Title = "Как стать программистом",
                Description = Strings.FirstSeederString,
                Author = "Илья Ильин",
                Date = new DateTime(2022, 8, 5),
                IsSeen = true,
                Category = MaterialCategory.Default,
            },
            new MaterialModel()
            {
                Title = "Как полюбить Путена",
                Description = Strings.FirstSeederString,
                Author = "Михаил Михайлов",
                Date = new DateTime(2022, 8, 5),
                IsSeen = true,
                Category = MaterialCategory.Default,
            },
            new MaterialModel()
            {
                Title = "Как понять, что ты не программист",
                Description = Strings.FirstSeederString,
                Author = "Руслан Арибжанов",
                Date = new DateTime(2022, 8, 5),
                IsSeen = true,
                Category = MaterialCategory.Default,
            },
            new MaterialModel()
            {
                Title = "Как сделать свой первый сайт",
                Description = Strings.FirstSeederString,
                Author = "Алишер Алишеров",
                Date = new DateTime(2022, 8, 5),
                IsSeen = true,
                Category = MaterialCategory.Default,
            },
            new MaterialModel()
            {
                Title = "Как понять что пора сменить работу",
                Description = Strings.FirstSeederString,
                Author = "Владимир Владимирович",
                Date = new DateTime(2022, 8, 5),
                IsSeen = true,
                Category = MaterialCategory.Default,
            },
            new MaterialModel()
            {
                Title = "Как украсть пароль от банковской карты",
                Description = Strings.FirstSeederString,
                Author = "Илона Маск",
                Date = new DateTime(2022, 8, 5),
                IsSeen = true,
                Category = MaterialCategory.Default,
            },
            new MaterialModel()
            {
                Title = "Как научится говорить дохуя",
                Description = Strings.FirstSeederString,
                Author = "Александр Кленин",
                Date = new DateTime(2022, 8, 5),
                IsSeen = true,
                Category = MaterialCategory.Default,
            },
            new MaterialModel()
            {
                Title = "Как правильно валить лес",
                Description = Strings.FirstSeederString,
                Author = "Иосиф Сталин",
                Date = new DateTime(2022, 8, 5),
                IsSeen = true,
                Category = MaterialCategory.Default,
            },
            new MaterialModel()
            {
                Title = "За что я люблю дубы",
                Description = Strings.FirstSeederString,
                Author = "Лев Толстой",
                Date = new DateTime(2022, 8, 5),
                IsSeen = true,
                Category = MaterialCategory.Default,
            },
        };
        _applicationContext.MaterialModels.AddRange(material);

        _applicationContext.SaveChanges();

        var photos = new List<ImageStorageModel>()
        {
            new ImageStorageModel()
            {
                userId = users.Last().id,
                imagePath = "boxes/ArcoLinux_2022-03-10_14-25-27.png",
                isUsed = true,
                imageType = ImageType.Task,
            },
            new ImageStorageModel()
            {
                userId = users.Last().id,
                imagePath = "boxes/ArcoLinux_2022-03-10_14-25-27.png",
                isUsed = true,
                imageType = ImageType.Task,
            },
            new ImageStorageModel()
            {
                userId = users.Last().id,
                imagePath = "boxes/ArcoLinux_2022-03-10_14-25-27.png",
                isUsed = true,
                imageType = ImageType.Task,
            },
            new ImageStorageModel()
            {
                userId = users.Last().id,
                imagePath = "boxes/image 1.png",
                isUsed = true,
                imageType = ImageType.Material,
            },
        };
        _applicationContext.ImageStorageModels.AddRange(photos);

        _applicationContext.SaveChanges();

        var materialImages = new List<MaterialImageModel>();
        material.ForEach(m =>
        {
            materialImages.Add(
                new MaterialImageModel()
                {
                    MaterialId = m.Id,
                    ImageId = photos.Last().id
                });
        });

        _applicationContext.MaterialImageModels.AddRange(materialImages);

        var users_tasks = new List<UserTaskModel>()
        {
            new UserTaskModel()
            {
                UserId = users.Last().id,
                TaskId = tasks.Last().Id,
                taskStatus = TaskStatus.OnCheck,
            },
        };
        _applicationContext.UserTaskModels.AddRange(users_tasks);
        _applicationContext.SaveChanges();

        var photosTask = new List<TaskImageModel>()
        {
            new TaskImageModel()
            {
                UserTaskId = users_tasks.Last().Id,
                imageId = photos[0].id
            },
            new TaskImageModel()
            {
                UserTaskId = users_tasks.Last().Id,
                imageId = photos[1].id
            },
            new TaskImageModel()
            {
                UserTaskId = users_tasks.Last().Id,
                imageId = photos[2].id
            },
        };
        _applicationContext.TaskImageModels.AddRange(photosTask);


        _applicationContext.SaveChanges();
    }
}