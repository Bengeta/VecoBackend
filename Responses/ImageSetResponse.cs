using SixLabors.ImageSharp;

namespace VecoBackend.Responses;

public class ImageSetResponse
{
    public List<string> ImagePaths { get; set; }
    public int UserTaskId { get; set; }
}