using SixLabors.ImageSharp;

namespace VecoBackend.Responses;

public class ImageSetResponse
{
    public List<Image> Images { get; set; }
    public int UserTaskId { get; set; }
}