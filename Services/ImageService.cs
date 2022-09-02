using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using VecoBackend.Data;
using VecoBackend.Interfaces;
using VecoBackend.Enums;
using VecoBackend.Models;
using VecoBackend.Responses;

namespace VecoBackend.Services;

public class ImageService
{
    private readonly IEnumerable<IImageProfile> _imageProfiles;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly string _connectionString;
    private ApplicationContext context;
    private NotificationService notificationService;

    public ImageService(IEnumerable<IImageProfile> imageProfiles, IWebHostEnvironment webHostEnvironment,
        IConfiguration configuration, NotificationService notificationService)
    {
        _connectionString = configuration.GetConnectionString("MainDB");
        if (_connectionString == null) throw new Exception("Connection string not specified");
        _imageProfiles = imageProfiles;
        _hostingEnvironment = webHostEnvironment;
        this.notificationService = notificationService;
    }

    public void AddContext(ApplicationContext _applicationContext)
    {
        context = _applicationContext;
    }

    public async Task<string> SaveImage(IFormFile file, int task_id, ImageType imageType)
    {
        var imageProfile = _imageProfiles.FirstOrDefault(profile =>
            profile.ImageType == imageType);

        if (imageProfile == null)
            throw new ImageProcessingException("Image profile has not found");

        ValidateExtension(file, imageProfile);
        ValidateFileSize(file, imageProfile);

        var image = Image.Load(file.OpenReadStream());

        ValidateImageSize(image, imageProfile);

        var folderPath = Path.Combine(_hostingEnvironment.WebRootPath, imageProfile.Folder);

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string filePath;
        string fileName;

        do
        {
            fileName = GenerateFileName(file);
            filePath = Path.Combine(folderPath, fileName);
        } while (File.Exists(filePath));

        Resize(image, imageProfile);
        Crop(image, imageProfile);
        image.Save(filePath, new JpegEncoder {Quality = 75});

        var fileUrl = Path.Combine(imageProfile.Folder, fileName);
        await SaveImageToDB(fileUrl, task_id);
        return fileUrl;
    }


    public async Task<Boolean> DeleteImageById(string token, int task_id, int image_id)
    {
        try
        {
            var user = await context.UserModels.FirstOrDefaultAsync(u =>
                u.id == context.UserTaskModels.FirstOrDefault(ut => ut.task_id == task_id)!.user_id);
            if (user.token != token && !user.isAdmin)
                return false;
            var image = await context.TaskPhotoModels.FindAsync(image_id);
            if (image == null)
                return false;
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, image.photoPath);
            if (File.Exists(filePath))
                File.Delete(filePath);
            context.TaskPhotoModels.Remove(image);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async Task<Boolean> DeleteImageTask(int taskId, string token = null)
    {
        try
        {
            if (token != null)
            {
                var user = await context.UserModels.FirstOrDefaultAsync(u =>
                    u.id == context.UserTaskModels.FirstOrDefault(ut => ut.task_id == taskId)!.user_id);
                if (user.token != token && !user.isAdmin)
                    return false;
            }

            var images = await context.TaskPhotoModels.Where(x => x.UserTaskId == taskId).ToListAsync();
            foreach (var image in images)
            {
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, image.photoPath);
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            context.TaskPhotoModels.RemoveRange(images);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async Task<List<string>> GetImageTask( int task_id,string token = null)
    {
        try
        {
            if (token != null)
            {
                var user = await context.UserModels.FirstOrDefaultAsync(u =>
                    u.id == context.UserTaskModels.FirstOrDefault(ut => ut.task_id == task_id)!.user_id);
                if (user.token != token && !user.isAdmin)
                    return null;
            }

            var images = await context.TaskPhotoModels.Where(x => x.UserTaskId == task_id).ToListAsync();
            var boxImages = new List<string>();
            foreach (var image in images)
            {
                boxImages.Add(image.photoPath);
            }

            return boxImages;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task<ImageSetResponse> CheckImageTask(int userTaskId)
    {
        try
        {
            var task = await context.UserTaskModels.Where(u => u.task_status == Enums.TaskStatus.OnCheck && u.id == userTaskId)
                .FirstOrDefaultAsync();
            if (task == null)
                return null;
            //task.task_status = Enums.TaskStatus.IsChecking;
            await context.SaveChangesAsync();
            var images = await GetImageTask(task.task_id);
            var ans = new ImageSetResponse()
            {
                ImagePaths = images,
                UserTaskId = task.task_id
            };
            return ans;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task AcceptTask( /*string token,*/ int task_id)
    {
        try
        {
            var user = await context.UserModels.FirstOrDefaultAsync(u =>
                u.id == context.UserTaskModels.FirstOrDefault(ut => ut.task_id == task_id)!.user_id);
            var task = await context.TaskModels.FirstOrDefaultAsync(u =>
                u.id == context.UserTaskModels.FirstOrDefault(ut => ut.task_id == task_id)!.task_id);
            var userTask = await context.UserTaskModels.FirstOrDefaultAsync(u => u.task_id == task_id);
            userTask.task_status = Enums.TaskStatus.Finished;
            user.points += task.points;
            //var ans = await CheckImageTask();
            await DeleteImageTask(task_id);
            await context.SaveChangesAsync();
            await Notificate(userTask.user_id, userTask.task_id, false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task DenyTask( /*string token, */ int task_id)
    {
        try
        {
            var userTask = await context.UserTaskModels.FirstOrDefaultAsync(u => u.task_id == task_id);
            userTask.task_status = Enums.TaskStatus.Created;
            //var ans = await CheckImageTask();
            await DeleteImageTask(task_id);
            await context.SaveChangesAsync();
            await Notificate(userTask.user_id, userTask.task_id, false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private async Task Notificate(int user_id, int task_id, Boolean isAccept)
    {
        try
        {
            var tokens = await context.NotificationTokensModels.Where(x => x.UserId == user_id).Select(u => u.Token)
                .ToListAsync();
            var task = await context.TaskModels.FindAsync(task_id);

            await NotificationService.Notify(tokens, isAccept, task.description);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    private void ValidateExtension(IFormFile file, IImageProfile imageProfile)
    {
        var fileExtension = Path.GetExtension(file.FileName);

        if (imageProfile.AllowedExtensions.Any(ext => ext == fileExtension.ToLower()))
            return;

        throw new ImageProcessingException();
    }

    private void ValidateFileSize(IFormFile file, IImageProfile imageProfile)
    {
        if (file.Length > imageProfile.MaxSizeBytes)
            throw new ImageProcessingException();
    }

    private void ValidateImageSize(Image image, IImageProfile imageProfile)
    {
        if (image.Width < imageProfile.Width || image.Height < imageProfile.Height)
            throw new ImageProcessingException();
    }

    private string GenerateFileName(IFormFile file)
    {
        var fileExtension = Path.GetExtension(file.FileName);
        var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());

        return $"{fileName}{fileExtension}";
    }

    private void Resize(Image image, IImageProfile imageProfile)
    {
        var resizeOptions = new ResizeOptions
        {
            Mode = ResizeMode.Min,
            Size = new Size(imageProfile.Width)
        };

        image.Mutate(action => action.Resize(resizeOptions));
    }

    private void Crop(Image image, IImageProfile imageProfile)
    {
        var rectangle = GetCropRectangle(image, imageProfile);
        image.Mutate(action => action.Crop(rectangle));
    }

    private Rectangle GetCropRectangle(IImageInfo image, IImageProfile imageProfile)
    {
        var widthDifference = image.Width - imageProfile.Width;
        var heightDifference = image.Height - imageProfile.Height;
        var x = widthDifference / 2;
        var y = heightDifference / 2;

        return new Rectangle(x, y, imageProfile.Width, imageProfile.Height);
    }


    private async Task<Boolean> SaveImageToDB(string filePath, int task_id)
    {
        try
        {
            var image = new TaskPhotoModel()
            {
                photoPath = filePath,
                UserTaskId = task_id
            };
            context.TaskPhotoModels.Add(image);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}