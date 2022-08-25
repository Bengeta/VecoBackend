using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.VisualBasic;
using SixLabors.ImageSharp;
using VecoBackend.Enums;
using VecoBackend.Services;

namespace VecoBackend.Controllers;

public class ImageController : ControllerBase
{
    private ImageService _imageService;

    public ImageController(ImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpPost]
    [Route("upload/box")]
    public IActionResult UploadBoxImage(IFormFile file)
    {
        return UploadImage(file, ImageType.Box);
    }

    [HttpPost]
    [Route("upload/logo")]
    public IActionResult UploadLogoImage(IFormFile file)
    {
        return UploadImage(file, ImageType.Logo);
    }

    private IActionResult UploadImage(IFormFile file, ImageType type)
    {
        if (file.Length == 0)
            return BadRequest("File is empty");

        try
        {
            var filePath = _imageService.SaveImage(file, type);
            return Ok();
        }
        catch (ImageProcessingException ex)
        {
            //var response = new ApiResponse(ErrorCodes.ImageProcessing, ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
           // var response = new ApiResponse(ErrorCodes.Unknown, ex.Message);
            return Ok(ex.Message);
        }
        return null;
    }
}