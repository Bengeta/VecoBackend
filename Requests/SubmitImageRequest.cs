namespace VecoBackend.Requests;

public class SubmitImageRequest
{
    public List<int> imageId { get; set; }
    public int taskId { get; set; }
}