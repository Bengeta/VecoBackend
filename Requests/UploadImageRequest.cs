
namespace VecoBackend.Responses;

public class UploadImageRequest
{
    public IFormFile file { get; set; }
    public int task_id { get; set; }
}