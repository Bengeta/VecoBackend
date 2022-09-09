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
        TimerCallback tm = new TimerCallback(DbClear);
        var time = (60- DateTime.Now.Minute) *60* 1000;
        Timer timer = new Timer(tm, null, time, 60*60 * 1000);
    }

    private async void DbClear(object? obj)
    {
        await DeleteOldTasks();
    }

    public async Task DeleteOldTasks()
    {
        try
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                var items = await (
                    from userTask in _context.UserTaskModels
                    join task in _context.TaskModels on userTask.task_id equals task.id
                    join taskImage in _context.TaskImageModels on userTask.id equals taskImage.UserTaskId
                    join images in _context.ImageStorageModels on taskImage.imageId equals images.id
                    where task.deadline <= DateTime.Now && userTask.task_status == TaskStatus.Created
                    orderby task.id
                    select new
                    {
                        imageId = images.id,
                        taskImageId = taskImage.id,
                        userTaskId = userTask.id,
                        taskId = task.id,
                        points = task.points,
                        title = task.title,
                        description = task.description,
                        type = task.type,
                        deadline = task.deadline,
                        imagePath = images.imagePath
                    }).ToListAsync();
                var prevTaskId = 0;
                var userTasks = new List<UserTaskModel>();
                var tasks = new List<TaskModel>();
                var imagesDel = new List<ImageStorageModel>();

                foreach (var item in items)
                {
                    userTasks.Add(new UserTaskModel() {id = item.userTaskId});
                    if (prevTaskId != item.taskId)
                        tasks.Add(new TaskModel()
                        {
                            id = item.taskId, isSeen = false, points = item.points, title = item.title,
                            description = item.description, type = item.type, deadline = item.deadline
                        });
                    prevTaskId = item.taskId;
                    imagesDel.Add(new ImageStorageModel() {id = item.imageId});
                    var filePath = Path.Combine(_hostEnvironment.WebRootPath, item.imagePath);
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
                _context.UserTaskModels.RemoveRange(userTasks);
                _context.TaskModels.UpdateRange(tasks);
                _context.ImageStorageModels.RemoveRange(imagesDel);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}