
namespace VecoBackend.Requests;

public class UploadImageRequest
{
    public IFormFile file { get; set; }
    public int taskId { get; set; }
}