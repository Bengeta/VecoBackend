using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SixLabors.ImageSharp;
using VecoBackend.Data;
using VecoBackend.Enums;
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
    public async Task<IActionResult> UploadBoxImage(UploadImageRequest request)
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        return await UploadImage(request, ImageType.Box, token);
    }

    [HttpPost]
    [Route("logo")]
    public async Task<IActionResult> UploadLogoImage(UploadImageRequest request)
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        return await UploadImage(request, ImageType.Logo, token);
    }

    [HttpDelete]
    [Route("images/{id}")]
    public async Task<IActionResult> DeleteImageById(DeleteImageRequest response, int imageId)
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var res = await _imageService.DeleteImageById(token, response.taskId, imageId);
        if (res)
            return Ok();
        return BadRequest();
    }

    [HttpDelete]
    [Route("images/all")]
    public async Task<IActionResult> DeleteImageTask(DeleteTaskImageRequest request)
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var res = await _imageService.DeleteImageTask(request.taskId, token);
        if (res)
            return Ok();
        return BadRequest();
    }

    [HttpGet]
    [Route("images/{taskId}")]
    public async Task<IActionResult> GetImageTask(int taskId)
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var res = await _imageService.GetImageTask(taskId, token);
        if (res != null)
            return Ok(res);
        return BadRequest();
    }


    private async Task<IActionResult> UploadImage(UploadImageRequest request, ImageType type, string token)
    {
        if (request.file.Length == 0)
            return BadRequest("File is empty");

        try
        {
            var imgId = await _imageService.SaveImage(request.file, request.task_id, type, token);
            if (imgId != -1)
                return Ok(imgId);
            return BadRequest();
        }
        catch (ImageProcessingException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return Ok(ex.Message);
        }
    }
}