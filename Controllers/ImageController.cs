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
public class ImageController : BaseController
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
    public async Task<ResponseModel<int>> UploadBoxImage([FromForm] UploadImageRequest request)
    {
        var token = Token();
        return await UploadImage(request, SaveImageType.Box, token);
    }

    /*
    [HttpPost]
    [Route("logo")]
    [RequestSizeLimit(4_000_000)]
    public async Task<ResponseModel<int>> UploadLogoImage(UploadImageRequest request)
    {
        var token = Token();
        return await UploadImage(request, SaveImageType.Logo, token);
    }*/


    /*[HttpGet]
    [Route("images/{id}")]
    public async Task<ResponseModel<List<string>>> GetImageById(int id)
    {
        var token = Token();
        var res = await _imageService.GetImageTask(id, token);
        if (res != null)
            return new ResponseModel<List<string>>() {ResultCode = ResultCode.Success, Data = res};
        return new ResponseModel<List<string>>() {ResultCode = ResultCode.Failed};
    }*/


    private async Task<ResponseModel<int>> UploadImage(UploadImageRequest request, SaveImageType type, string token)
    {
        if (request.file.Length == 0)
            return new ResponseModel<int>() {ResultCode = ResultCode.FileException};

        try
        {
            return await _imageService.SaveImage(request.file, type, token);
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