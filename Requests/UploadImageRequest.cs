
namespace VecoBackend.Requests;

public class UploadImageRequest
{
    public IFormFile file { get; set; }
    public int TaskId { get; set; }
}