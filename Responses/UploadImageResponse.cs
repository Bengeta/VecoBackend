
namespace VecoBackend.Responses;

public class UploadImageResponse
{
    public IFormFile file { get; set; }
    public int task_id { get; set; }
}