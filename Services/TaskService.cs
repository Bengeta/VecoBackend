using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using VecoBackend.Data;
using VecoBackend.Enums;
using VecoBackend.Models;
using VecoBackend.Responses;
using VecoBackend.Utils;
using TaskStatus = VecoBackend.Enums.TaskStatus;


namespace VecoBackend.Services;

public class TaskService
{
    private ApplicationContext _context;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public TaskService(IWebHostEnvironment webHostEnvironment, IWebHostEnvironment hostingEnvironment)
    {
        _hostingEnvironment = hostingEnvironment;
    }

    public void AddContext(ApplicationContext applicationContext)
    {
        _context = applicationContext;
    }

    public async Task<List<GetTaskResponse>> GetAllTasks(string token)
    {
        try
        {
            var startTask = await (from user in _context.UserModels
                join userTask in _context.UserTaskModels on user.id equals userTask.UserId
                join task in _context.TaskModels on userTask.TaskId equals task.Id
                where user.token == token && userTask.taskStatus != TaskStatus.Created && task.IsSeen
                select new
                {
                    id = task.Id,
                    points = task.Points,
                    title = task.Title,
                    description = task.Description,
                    type = task.Type,
                    isSeen = task.IsSeen,
                    deadline = task.Deadline,
                    status = userTask.taskStatus
                }).ToListAsync();
            var answer = new List<GetTaskResponse>();
            var startTaskId = new List<int>();
            startTask.ForEach(task =>
            {
                startTaskId.Add(task.id);
                answer.Add(new GetTaskResponse()
                {
                    Id = task.id,
                    Points = task.points,
                    Title = task.title,
                    Description = task.description,
                    Type = task.type,
                    IsSeen = task.isSeen,
                    Deadline = Converter.ToUnixTime(task.deadline),
                    Status = task.status
                });
            });
            var notStartedTasks =
                await _context.TaskModels.Where(x => x.IsSeen && !startTaskId.Contains(x.Id)).ToListAsync();
            answer.AddRange(ConvertToGetTaskResponse(notStartedTasks));

            return answer;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<GetTaskResponse>> GetTasks(string token, TaskStatus status)
    {
        try
        {
            List<TaskModel> tasks;
            if (status == TaskStatus.Created)
                tasks = await _context.TaskModels.Where(x => x.IsSeen).ToListAsync();
            else
                tasks = await (from user in _context.UserModels
                    join userTask in _context.UserTaskModels on user.id equals userTask.UserId
                    join task in _context.TaskModels on userTask.TaskId equals task.Id
                    where user.token == token && userTask.taskStatus == status
                    select task).ToListAsync();
            var answer = new List<GetTaskResponse>();
            answer.AddRange(ConvertToGetTaskResponse(tasks));
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
                    join userTask in _context.UserTaskModels on user.id equals userTask.UserId
                    join task in _context.TaskModels on userTask.TaskId equals task.Id
                    where user.token == token && task.Id == taskId
                    select userTask).FirstOrDefaultAsync();
            if (UserTask == null)
                return new ResponseModel<int>() {ResultCode = ResultCode.TaskNotFound};
            UserTask.taskStatus = newStatus;
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
                join userTask in _context.UserTaskModels on user.id equals userTask.UserId
                join task in _context.TaskModels on userTask.TaskId equals task.Id
                where userTask.taskStatus == TaskStatus.OnCheck
                select new
                {
                    id = userTask.Id,
                    points = task.Points,
                    title = task.Title,
                    description = task.Description,
                    type = task.Type,
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

    public async Task<GetTaskResponse> GetTaskById(int id)
    {
        try
        {
            var task = await _context.TaskModels.Where(u => u.Id == id).FirstOrDefaultAsync();
            return ConvertToGetTaskResponse(task);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task AddTask(AddTaskResponse task)
    {
        try
        {
            var newTask = new TaskModel()
            {
                Title = task.Title,
                Description = task.Description,
                Type = task.Type,
                Points = task.Points,
                Deadline = task.Deadline,
                IsSeen = false,
            };
            await _context.TaskModels.AddAsync(newTask);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task ChangeTaskVisibility(int taskId, bool visibility)
    {
        try
        {
            var task = await _context.TaskModels.Where(u => u.Id == taskId).FirstOrDefaultAsync();
            task.IsSeen = visibility;
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task DeleteTask(int taskId)
    {
        try
        {
            var task = await _context.TaskModels.Where(u => u.Id == taskId).FirstOrDefaultAsync();
            _context.TaskModels.Remove(task);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task ChangeTask(ChangeTaskResponse task)
    {
        try
        {
            var newTask = await _context.TaskModels.Where(u => u.Id == task.Id).FirstOrDefaultAsync();
            newTask.Title = task.Title;
            newTask.Description = task.Description;
            newTask.Type = task.Type;
            newTask.Points = task.Points;
            newTask.Deadline = task.Deadline;
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<TaskModel>> GetTasksByType(TaskType type)
    {
        try
        {
            var tasks = await _context.TaskModels.Where(u => u.Type == type).ToListAsync();
            return tasks;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    public async Task<ResultCode> SubmitImages(List<int> imagesId, int taskId, string token)
    {
        try
        {
            var images =
                await (from user in _context.UserModels
                    join image in _context.ImageStorageModels on user.id equals image.userId
                    where user.token == token && !image.isUsed
                    select image).ToListAsync();
            var task = await _context.TaskModels.Where(u => u.Id == taskId).FirstOrDefaultAsync();
            if(task == null)
                return ResultCode.TaskNotFound;
            var userTask = await _context.UserTaskModels.FirstOrDefaultAsync(u =>
                u.UserId == images[0].userId && u.TaskId == taskId);
            if (userTask == null)
            {
                userTask = new UserTaskModel()
                {
                    TaskId = taskId,
                    UserId = images[0].userId,
                    taskStatus = Enums.TaskStatus.OnCheck,
                    user = images[0].UserModel,
                    task = task,
                };
                _context.UserTaskModels.Add(userTask);
            }
            else
            {
                userTask.taskStatus = Enums.TaskStatus.OnCheck;
                userTask.TaskId = taskId;
                userTask.UserId = images[0].userId;
                _context.UserTaskModels.Update(userTask);
            }


            foreach (var image in images)
            {
                if (imagesId.Contains(image.id))
                {
                    _context.TaskImageModels.Add(new TaskImageModel()
                    {
                        imageId = image.id,
                        UserTaskId = userTask.Id,
                        ImageStorage = image,
                        UserTask = userTask
                    });
                    image.isUsed = true;
                    _context.ImageStorageModels.Update(image);
                }
                else
                {
                    _context.ImageStorageModels.Remove(image);

                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, image.imagePath);
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
            }

            ;

            await _context.SaveChangesAsync();
            return ResultCode.Success;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ResultCode.Failed;
        }
    }

    private List<GetTaskResponse> ConvertToGetTaskResponse(List<TaskModel> tasks)
    {
        var answer = new List<GetTaskResponse>();
        tasks.ForEach(task =>
        {
            answer.Add(new GetTaskResponse()
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Type = task.Type,
                Points = task.Points,
                Deadline = Converter.ToUnixTime(task.Deadline),
                IsSeen = task.IsSeen,
            });
        });
        return answer;
    }

    private GetTaskResponse ConvertToGetTaskResponse(TaskModel task)
    {
        return new GetTaskResponse()
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Type = task.Type,
            Points = task.Points,
            Deadline = Converter.ToUnixTime(task.Deadline),
            IsSeen = task.IsSeen,
        };
    }
}