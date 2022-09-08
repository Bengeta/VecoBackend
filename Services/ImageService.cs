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

    public async Task<ResponseModel<int>> SaveImage(IFormFile file, ImageType imageType, string token,int taskId)
    {
        var imageProfile = _imageProfiles.FirstOrDefault(profile =>
            profile.ImageType == imageType);

        if (imageProfile == null)
            return new ResponseModel<int> {ResultCode = ResultCode.Failed};

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
        var imgId = await SaveImageToDb(fileUrl, token,taskId);
        if (imgId > -1)
            return new ResponseModel<int>() {ResultCode = ResultCode.Success, Data = imgId};
        return new ResponseModel<int>() {ResultCode = ResultCode.Failed};
    }


    public async Task<ResultCode> DeleteImageById(string token, int taskId, int imageId)
    {
        try
        {
            var img =
                await (from usr in _context.UserModels
                    join userTask in _context.UserTaskModels on usr.id equals userTask.user_id
                    join task in _context.TaskModels on userTask.task_id equals task.id
                    join imageTask in _context.TaskImageModels on userTask.id equals imageTask.UserTaskId
                    join image in _context.ImageStorageModels on imageTask.imageId equals image.id
                    where (usr.token == token || usr.isAdmin) && task.id == taskId && imageTask.id == imageId
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
                    join userTask in _context.UserTaskModels on usr.id equals userTask.user_id
                    join task in _context.TaskModels on userTask.task_id equals task.id
                    join imageTask in _context.TaskImageModels on userTask.id equals imageTask.UserTaskId
                    join image in _context.ImageStorageModels on imageTask.imageId equals image.id
                    where (usr.token == token || usr.isAdmin) && userTask.id == userTaskId
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
                    join userTask in _context.UserTaskModels on usr.id equals userTask.user_id
                    join task in _context.TaskModels on userTask.task_id equals task.id
                    join photos in _context.TaskImageModels on userTask.id equals photos.UserTaskId
                    join image in _context.ImageStorageModels on photos.imageId equals image.id
                    where (usr.token == token || usr.isAdmin) && userTask.id == userTaskId
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
                .Where(u => u.task_status == Enums.TaskStatus.OnCheck && u.id == userTaskId)
                .FirstOrDefaultAsync();
            if (task == null)
                return null;
            //task.task_status = Enums.TaskStatus.IsChecking;
            await _context.SaveChangesAsync();
            var images = await GetImageTask(task.id);
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

    public async Task AcceptTask(int userTaskId)
    {
        try
        {
            var user = await _context.UserModels.FirstOrDefaultAsync(u =>
                u.id == _context.UserTaskModels.FirstOrDefault(ut => ut.id == userTaskId)!.user_id);
            var task = await _context.TaskModels.FirstOrDefaultAsync(u =>
                u.id == _context.UserTaskModels.FirstOrDefault(ut => ut.id == userTaskId)!.task_id);
            var userTask = await _context.UserTaskModels.FirstOrDefaultAsync(u => u.id == userTaskId);
            userTask.task_status = Enums.TaskStatus.Finished;
            user.points += task.points;
            await DeleteImageTask(userTaskId);
            await _context.SaveChangesAsync();
            await Notificate(userTask.user_id, userTask.task_id, false);
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
            var userTask = await _context.UserTaskModels.FirstOrDefaultAsync(u => u.id == userTaskId);
            if (userTask != null)
            {
                userTask.task_status = Enums.TaskStatus.Created;
                await DeleteImageTask(userTaskId);
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


    public async Task<ResultCode> SubmitImages(List<int> imagesId, int taskId, string token)
    {
        try
        {
            var images =
                await (from user in _context.UserModels
                    join userTask in _context.UserTaskModels on user.id equals userTask.user_id
                    join task in _context.TaskModels on userTask.task_id equals task.id
                    join image in _context.ImageStorageModels on user.id equals image.userId
                    where user.token == token && task.id == taskId
                    select new
                    {
                        userTaskId = userTask.id,
                        id = image.id,
                        imagePath = image.imagePath,
                        userId = user.id
                    }).ToListAsync();
            var imagesSubmit = new List<TaskImageModel>();
            var imagesDelete = new List<ImageStorageModel>();
            images.ForEach(u =>
            {
                if (imagesId.Contains(u.id))
                {
                    imagesSubmit.Add(new TaskImageModel()
                    {
                        imageId = u.id,
                        UserTaskId = u.userTaskId
                    });
                }
                else
                {
                    imagesDelete.Add(new ImageStorageModel()
                    {
                        id = u.id,
                        imagePath = u.imagePath,
                        userId = u.userId
                    });

                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, u.imagePath);
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
            });
            _context.TaskImageModels.AddRange(imagesSubmit);
            _context.ImageStorageModels.RemoveRange(imagesDelete);
            await _context.SaveChangesAsync();
            return ResultCode.Success;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ResultCode.Failed;
        }
    }


    private async Task<int> SaveImageToDb(string filePath, string token,int taskId)
    {
        try
        {
            var userId = await _context.UserModels.Where(u => u.token == token).Select(u => u.id).FirstOrDefaultAsync();
            var isUserTask = await _context.UserTaskModels.AnyAsync(u => u.user_id == userId && u.task_id == taskId);
            var image = new ImageStorageModel()
            {
                imagePath = filePath,
                userId = userId
            };
            if(!isUserTask)
                _context.UserTaskModels.Add(new UserTaskModel()
                {
                    task_id = taskId,
                    user_id = userId,
                    task_status = Enums.TaskStatus.Created
                });
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