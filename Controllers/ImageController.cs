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
    public async Task<IActionResult> UploadBoxImage(UploadImageResponse response)
    {
        return await UploadImage(response, ImageType.Box);
    }

    [HttpPost]
    [Route("upload/logo")]
    public async Task<IActionResult> UploadLogoImage(UploadImageResponse response)
    {
        return await UploadImage(response, ImageType.Logo);
    }

    private async Task<IActionResult> UploadImage(UploadImageResponse response, ImageType type)
    {
        if (response.file.Length == 0)
            return BadRequest("File is empty");

        try
        {
            var filePath = await _imageService.SaveImage(response.file, response.task_id, type);
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