using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.VisualBasic;
using SixLabors.ImageSharp;
using VecoBackend.Data;
using VecoBackend.Enums;
using VecoBackend.Responses;
using VecoBackend.Services;

namespace VecoBackend.Controllers;

public class ImageController : ControllerBase
{
    private ImageService _imageService;

    public ImageController(ImageService imageService,ApplicationContext context)
    {
        _imageService = imageService;
        imageService.AddContext(context);
    }

    [HttpPost]
    [Route("upload/box")]
    public async Task<IActionResult> UploadBoxImage(UploadImageRequest request)
    {
        
        var token = "asdf";//Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        return await UploadImage(request, ImageType.Box);
    }

    [HttpPost]
    [Route("upload/logo")]
    public async Task<IActionResult> UploadLogoImage(UploadImageRequest request)
    {
        return await UploadImage(request, ImageType.Logo);
    }
    [HttpDelete]
    [Route("delete/id")]
    public async Task<IActionResult> DeleteImageById(DeleteImageRequest response)
    {
        var token = "asdf";//Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var res = await _imageService.DeleteImageById(token,response.task_id,response.image_id);
        if (res)
            return Ok();
        return BadRequest();
    }
    
    [HttpDelete]
    [Route("delete/task")]
    public async Task<IActionResult> DeleteImageTask(DeleteTaskImageRequest response)
    {
        var token = "asdf";//Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var res = await _imageService.DeleteImageTask(token,response.task_id);
        if (res)
            return Ok();
        return BadRequest();
    }
    
    [HttpGet]
    [Route("get/task")]
    public async Task<IActionResult> GetImageTask(GetImageTaskRequest request)
    {
        var token = "asdf";//Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var res = await _imageService.GetImageTask(token,request.task_id);
        if (res != null)
            return Ok(res);
        return BadRequest();
    }
    
    
    

    private async Task<IActionResult> UploadImage(UploadImageRequest request, ImageType type)
    {
        if (request.file.Length == 0)
            return BadRequest("File is empty");

        try
        {
            var filePath = await _imageService.SaveImage(request.file, request.task_id, type);
            return Ok();
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