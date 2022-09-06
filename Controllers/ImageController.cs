using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SixLabors.ImageSharp;
using VecoBackend.Data;
using VecoBackend.Enums;
using VecoBackend.Models;
using VecoBackend.Requests;
using VecoBackend.Responses;
using VecoBackend.Services;

namespace VecoBackend.Controllers;

[ApiController]
[Authorize]
public class ImageController : ControllerBase
{
    private ImageService _imageService;
    public ImageController(ImageService imageService, ApplicationContext context)
    {
        _imageService = imageService;
        imageService.AddContext(context);
    }

    [HttpPost]
    [Route("box")]
    [RequestSizeLimit(4_000_000)]
    public async Task<ResponseModel<int>> UploadBoxImage(UploadImageRequest request)
    {
        var token = HttpContext.Items["Token"].ToString();
        return await UploadImage(request, ImageType.Box, token);
    }

    [HttpPost]
    [Route("logo")]
    [RequestSizeLimit(4_000_000)]
    public async Task<ResponseModel<int>> UploadLogoImage(UploadImageRequest request)
    {
        var token = HttpContext.Items["Token"].ToString();
        return await UploadImage(request, ImageType.Logo, token);
    }

    [HttpDelete]
    [Route("images/{id}")]
    public async Task<ResponseModel<string>> DeleteImageById(DeleteImageRequest response, int id)
    {
        var token = HttpContext.Items["Token"].ToString();
        var res = await _imageService.DeleteImageById(token, response.taskId, id);
        var answer = new ResponseModel<string>() {ResultCode = res};
        return answer;
    }

    [HttpDelete]
    [Route("images/all")]
    public async Task<ResponseModel<string>> DeleteImageTask(DeleteTaskImageRequest request)
    {
        var token = HttpContext.Items["Token"].ToString();
        var res = await _imageService.DeleteImageTask(request.taskId, token);
        var answer = new ResponseModel<string>() {ResultCode = res};
        return answer;
    }

    [HttpGet]
    [Route("images/{taskId}")]
    public async Task<ResponseModel<List<string>>> GetImageTask(int taskId)
    {
        var token = HttpContext.Items["Token"].ToString();
        var res = await _imageService.GetImageTask(taskId, token);
        if (res != null)
            return new ResponseModel<List<string>>() {ResultCode = ResultCode.Success, Data = res};
        return new ResponseModel<List<string>>() {ResultCode = ResultCode.Failed};
    }


    private async Task<ResponseModel<int>> UploadImage(UploadImageRequest request, ImageType type, string token)
    {
        if (request.file.Length == 0)
            return new ResponseModel<int>() {ResultCode = ResultCode.FileException};

        try
        {
            return await _imageService.SaveImage(request.file, request.TaskId, type, token);
        }
        catch (ImageProcessingException ex)
        {
            return new ResponseModel<int>() {ResultCode = ResultCode.Failed};
        }
        catch (Exception ex)
        {
            return new ResponseModel<int>() {ResultCode = ResultCode.Failed};
        }
    }
}