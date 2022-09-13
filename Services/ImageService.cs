using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Webp;
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

    public async Task<ResponseModel<int>> SaveImage(IFormFile file, SaveImageType imageType, string token)
    {
        try
        {
            var imageProfile = _imageProfiles.FirstOrDefault(profile =>
                profile.ImageType == imageType);

            if (imageProfile == null)
                return new ResponseModel<int> {ResultCode = ResultCode.Failed};

            ValidateExtension(file, imageProfile);
            ValidateFileSize(file, imageProfile);

            var image = Image.Load(file.OpenReadStream());

            //ValidateImageSize(image, imageProfile);

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

            image.Save(filePath, new WebpEncoder());
            

            var fileUrl = Path.Combine(imageProfile.Folder, fileName);
            var imgId = await SaveImageToDb(fileUrl, token);
            if (imgId > -1)
                return new ResponseModel<int>() {ResultCode = ResultCode.Success, Data = imgId};
            return new ResponseModel<int>() {ResultCode = ResultCode.Failed};
        }
        catch (Exception e)
        {
            return new ResponseModel<int>() {ResultCode = ResultCode.Failed};
        }
    }


    public async Task<ResultCode> DeleteImageById(string token, int taskId, int imageId)
    {
        try
        {
            var img =
                await (from usr in _context.UserModels
                    join userTask in _context.UserTaskModels on usr.id equals userTask.UserId
                    join task in _context.TaskModels on userTask.TaskId equals task.Id
                    join imageTask in _context.TaskImageModels on userTask.Id equals imageTask.UserTaskId
                    join image in _context.ImageStorageModels on imageTask.imageId equals image.id
                    where (usr.token == token || usr.isAdmin) && task.Id == taskId && imageTask.id == imageId
                    select new
                    {
                        imageTaskId = imageTask.id,
                        imageId = image.id,
                        imagePath = image.imagePath
                    }).FirstOrDefaultAsync();

            if (img == null)
                return ResultCode.Failed;
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, img.imagePath);
            if (File.Exists(filePath))
                File.Delete(filePath);

            var taskImage = new TaskImageModel() {id = img.imageTaskId};
            _context.TaskImageModels.Remove(taskImage);
            var imageStorage = new ImageStorageModel() {id = img.imageId};
            _context.ImageStorageModels.Remove(imageStorage);
            await _context.SaveChangesAsync();
            return ResultCode.Success;
        }
        catch (Exception e)
        {
            return ResultCode.Failed;
        }
    }

    public async Task<ResultCode> DeleteImageTask(int userTaskId, string token = "asdf")
    {
        try
        {
            var images =
                await (from usr in _context.UserModels
                    join userTask in _context.UserTaskModels on usr.id equals userTask.UserId
                    join task in _context.TaskModels on userTask.TaskId equals task.Id
                    join imageTask in _context.TaskImageModels on userTask.Id equals imageTask.UserTaskId
                    join image in _context.ImageStorageModels on imageTask.imageId equals image.id
                    where (usr.token == token || usr.isAdmin) && userTask.Id == userTaskId
                    select image).ToListAsync();
            var imageTasks =
                await (from imageTask in _context.TaskImageModels
                    join image in _context.ImageStorageModels on imageTask.imageId equals image.id
                    where imageTask.UserTaskId == userTaskId
                    select imageTask).ToListAsync();

            foreach (var image in images)
            {
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, image.imagePath);
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            _context.TaskImageModels.RemoveRange(imageTasks);
            _context.ImageStorageModels.RemoveRange(images);
            await _context.SaveChangesAsync();
            return ResultCode.Success;
        }
        catch (Exception e)
        {
            return ResultCode.Failed;
        }
    }

    public async Task<List<string>> GetImageTask(int userTaskId, string token = "asdf")
    {
        try
        {
            var images =
                await (from usr in _context.UserModels
                    join userTask in _context.UserTaskModels on usr.id equals userTask.UserId
                    join task in _context.TaskModels on userTask.TaskId equals task.Id
                    join photos in _context.TaskImageModels on userTask.Id equals photos.UserTaskId
                    join image in _context.ImageStorageModels on photos.imageId equals image.id
                    where (usr.token == token || usr.isAdmin) && userTask.Id == userTaskId
                    select image).ToListAsync();
            var boxImages = new List<string>();
            foreach (var image in images)
            {
                boxImages.Add(image.imagePath);
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
                .Where(u => u.taskStatus == Enums.TaskStatus.OnCheck && u.Id == userTaskId)
                .FirstOrDefaultAsync();
            if (task == null)
                return null;
            //task.taskStatus = Enums.TaskStatus.IsChecking;
            await _context.SaveChangesAsync();
            var images = await GetImageTask(task.Id);
            var ans = new ImageSetResponse()
            {
                ImagePaths = images,
                UserTaskId = task.TaskId
            };
            return ans;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task AcceptTask(int userTaskId)
    {
        try
        {
            var user = await _context.UserModels.FirstOrDefaultAsync(u =>
                u.id == _context.UserTaskModels.FirstOrDefault(ut => ut.Id == userTaskId)!.UserId);
            var task = await _context.TaskModels.FirstOrDefaultAsync(u =>
                u.Id == _context.UserTaskModels.FirstOrDefault(ut => ut.Id == userTaskId)!.TaskId);
            var userTask = await _context.UserTaskModels.FirstOrDefaultAsync(u => u.Id == userTaskId);
            userTask.taskStatus = Enums.TaskStatus.Finished;
            user.points += task.Points;
            await DeleteImageTask(userTaskId);
            await _context.SaveChangesAsync();
            await Notificate(userTask.UserId, userTask.TaskId, false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task DenyTask(int userTaskId)
    {
        try
        {
            var userTask = await _context.UserTaskModels.FirstOrDefaultAsync(u => u.Id == userTaskId);
            if (userTask != null)
            {
                userTask.taskStatus = Enums.TaskStatus.Created;
                await DeleteImageTask(userTaskId);
                await _context.SaveChangesAsync();
                await Notificate(userTask.UserId, userTask.TaskId, false);
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

            await NotificationService.Notify(tokens, isAccept, task.Description);
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
        var fileName = Guid.NewGuid().ToString();

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


    private async Task<int> SaveImageToDb(string filePath, string token)
    {
        try
        {
            var user = await _context.UserModels.Where(u => u.token == token).FirstOrDefaultAsync();
            var image = new ImageStorageModel()
            {
                userId = user.id,
                UserModel = user,
                imagePath = filePath,
                imageType = ImageType.Task,
                
            };
            _context.ImageStorageModels.Add(image);
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