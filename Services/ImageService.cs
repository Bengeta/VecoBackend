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
    private ApplicationContext _context;
    private NotificationService _notificationService;

    public ImageService(IEnumerable<IImageProfile> imageProfiles, IWebHostEnvironment webHostEnvironment,
        NotificationService notificationService)
    {
        _imageProfiles = imageProfiles;
        _hostingEnvironment = webHostEnvironment;
        this._notificationService = notificationService;
    }

    public void AddContext(ApplicationContext applicationContext)
    {
        _context = applicationContext;
    }

    public async Task<int> SaveImage(IFormFile file, int taskId, ImageType imageType, string token)
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
        var imgId = await SaveImageToDb(fileUrl, taskId, token);
        return imgId;
    }


    public async Task<Boolean> DeleteImageById(string token, int taskId, int imageId)
    {
        try
        {
            var img =
                await (from usr in _context.UserModels
                    join userTask in _context.UserTaskModels on usr.id equals userTask.user_id
                    join task in _context.TaskModels on userTask.task_id equals task.id
                    join photos in _context.TaskPhotoModels on userTask.id equals photos.UserTaskId 
                    where (usr.token == token || usr.isAdmin) && task.id == taskId && photos.id == imageId
                    select photos).FirstOrDefaultAsync();
            if (img == null)
                return false;
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, img.photoPath);
            if (File.Exists(filePath))
                File.Delete(filePath);
            _context.TaskPhotoModels.Remove(img);
            await _context.SaveChangesAsync();
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
                var user = await _context.UserModels.FirstOrDefaultAsync(u =>
                    u.id == _context.UserTaskModels.FirstOrDefault(ut => ut.task_id == taskId)!.user_id);
                if (user.token != token && !user.isAdmin)
                    return false;
            }

            var images = await _context.TaskPhotoModels.Where(x => x.UserTaskId == taskId).ToListAsync();
            foreach (var image in images)
            {
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, image.photoPath);
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            _context.TaskPhotoModels.RemoveRange(images);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async Task<List<string>> GetImageTask(int taskId, string token = null)
    {
        try
        {
            if (token != null)
            {
                var user = await _context.UserModels.FirstOrDefaultAsync(u =>
                    u.id == _context.UserTaskModels.FirstOrDefault(ut => ut.task_id == taskId)!.user_id);
                if (user.token != token && !user.isAdmin)
                    return null;
            }

            var images = await _context.TaskPhotoModels.Where(x => x.UserTaskId == taskId).ToListAsync();
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
            var task = await _context.UserTaskModels
                .Where(u => u.task_status == Enums.TaskStatus.OnCheck && u.id == userTaskId)
                .FirstOrDefaultAsync();
            if (task == null)
                return null;
            //task.task_status = Enums.TaskStatus.IsChecking;
            await _context.SaveChangesAsync();
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

    public async Task AcceptTask( /*string token,*/ int taskId)
    {
        try
        {
            var user = await _context.UserModels.FirstOrDefaultAsync(u =>
                u.id == _context.UserTaskModels.FirstOrDefault(ut => ut.task_id == taskId)!.user_id);
            var task = await _context.TaskModels.FirstOrDefaultAsync(u =>
                u.id == _context.UserTaskModels.FirstOrDefault(ut => ut.task_id == taskId)!.task_id);
            var userTask = await _context.UserTaskModels.FirstOrDefaultAsync(u => u.task_id == taskId);
            userTask.task_status = Enums.TaskStatus.Finished;
            user.points += task.points;
            //var ans = await CheckImageTask();
            await DeleteImageTask(taskId);
            await _context.SaveChangesAsync();
            await Notificate(userTask.user_id, userTask.task_id, false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task DenyTask( /*string token, */ int taskId)
    {
        try
        {
            var userTask = await _context.UserTaskModels.FirstOrDefaultAsync(u => u.task_id == taskId);
            if (userTask != null)
            {
                userTask.task_status = Enums.TaskStatus.Created;
                //var ans = await CheckImageTask();
                await DeleteImageTask(taskId);
                await _context.SaveChangesAsync();
                await Notificate(userTask.user_id, userTask.task_id, false);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private async Task Notificate(int userId, int taskId, Boolean isAccept)
    {
        try
        {
            var tokens = await _context.NotificationTokensModels.Where(x => x.UserId == userId).Select(u => u.Token)
                .ToListAsync();
            var task = await _context.TaskModels.FindAsync(taskId);

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


    private async Task<int> SaveImageToDb(string filePath, int taskId, string token)
    {
        try
        {
            var userTaskId =
                await (from user in _context.UserModels
                    join userTask in _context.UserTaskModels on user.id equals userTask.user_id
                    join task in _context.TaskModels on userTask.task_id equals task.id
                    where user.token == token && task.id == taskId
                    select userTask.id).FirstOrDefaultAsync();
            var image = new TaskPhotoModel()
            {
                photoPath = filePath,
                UserTaskId = userTaskId
            };
            _context.TaskPhotoModels.Add(image);
            await _context.SaveChangesAsync();
            return image.id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return -1;
        }
    }
}