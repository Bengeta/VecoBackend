using Microsoft.AspNetCore.Components.Forms;
using VecoBackend.Enums;
using VecoBackend.Models;
using VecoBackend.Responses;

namespace VecoBackend.Interfaces;

public interface IImageService
{
    public Task<ResponseModel<int>> SaveImage(IFormFile file, SaveImageType imageType, string token);
    public Task<ResponseModel<ImageResponseModel>> SaveImage(IBrowserFile file, SaveImageType imageType, string token);
    public Task<ResultCode> DeleteImageById(string token, int taskId, int imageId);
    public Task<ResultCode> DeleteImageTask(int userTaskId);
    public Task<List<string>> GetImageTask(int userTaskId);
    public Task<ImageSetResponse> CheckImageTask(int userTaskId);
    public Task AcceptTask(int userTaskId);
    public Task DenyTask(int userTaskId);
    public Task DeleteImageMaterials(string token);
}