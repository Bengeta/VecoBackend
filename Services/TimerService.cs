using Microsoft.EntityFrameworkCore;
using VecoBackend.Data;
using VecoBackend.Models;
using TaskStatus = VecoBackend.Enums.TaskStatus;

namespace VecoBackend.Services;

public class TimerService
{
    private readonly IDbContextFactory<ApplicationContext> _contextFactory;
    private readonly IWebHostEnvironment _hostEnvironment;

    public TimerService(TaskService taskService, IDbContextFactory<ApplicationContext> contextFactory,
        IWebHostEnvironment hostingEnvironment)
    {
        _contextFactory = contextFactory;
        _hostEnvironment = hostingEnvironment;
    }

    public void Start()
    {
        var timerCallback1 = new TimerCallback(DeleteOldTasks);
        var time = (60 - DateTime.Now.Minute) * 60 * 1000;
        new Timer(timerCallback1, null, time, 60 * 60 * 1000);
        
        var timerCallback2 = new TimerCallback(DeleteFinishedTasks);
        var day = 24 * 60 * 60 * 1000;
        time = (day - DateTime.Now.Millisecond) + 7 * day;
        new Timer(timerCallback2, null, time, day);
    }

    private async void DeleteOldTasks(object? obj)
    {
        await DeleteOldTasks();
    }

    private async void DeleteFinishedTasks(object? obj)
    {
        await DeleteFinishedTask();
    }

    public async Task DeleteOldTasks()
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var items = await (
                from userTask in context.UserTaskModels
                join task in context.TaskModels on userTask.TaskId equals task.Id
                join taskImage in context.TaskImageModels on userTask.Id equals taskImage.UserTaskId
                join images in context.ImageStorageModels on taskImage.imageId equals images.id
                where task.Deadline <= DateTime.Now && userTask.taskStatus == TaskStatus.Created
                orderby task.Id
                select new
                {
                    imageId = images.id,
                    taskImageId = taskImage.id,
                    userTaskId = userTask.Id,
                    taskId = task.Id,
                    points = task.Points,
                    title = task.Title,
                    description = task.Description,
                    type = task.Type,
                    deadline = task.Deadline,
                    imagePath = images.imagePath
                }).ToListAsync();
            var prevTaskId = 0;
            var userTasks = new List<UserTaskModel>();
            var tasks = new List<TaskModel>();
            var imagesDel = new List<ImageStorageModel>();

            foreach (var item in items)
            {
                userTasks.Add(new UserTaskModel() {Id = item.userTaskId});
                if (prevTaskId != item.taskId)
                    tasks.Add(new TaskModel()
                    {
                        Id = item.taskId, IsSeen = false, Points = item.points, Title = item.title,
                        Description = item.description, Type = item.type, Deadline = item.deadline
                    });
                prevTaskId = item.taskId;
                imagesDel.Add(new ImageStorageModel() {id = item.imageId});
                var filePath = Path.Combine(_hostEnvironment.WebRootPath, item.imagePath);
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            context.UserTaskModels.RemoveRange(userTasks);
            context.TaskModels.UpdateRange(tasks);
            context.ImageStorageModels.RemoveRange(imagesDel);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task DeleteFinishedTask()
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var userTasks = await context.UserTaskModels
                .Where(x => x.taskStatus == TaskStatus.Finished && x.DeleteTime <= DateTime.Today)
                .ToListAsync();

            context.UserTaskModels.RemoveRange(userTasks);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}