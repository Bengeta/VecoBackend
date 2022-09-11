using VecoBackend.Enums;

namespace VecoBackend.Responses;

public class MaterialResponse
{
    public int Id { get; set; }
    public string Author { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public long Date { get; set; }
    public List<string> imagePaths { get; set; }
    public List<ImageType> imageTypes { get; set; }
    public MaterialCategory Category { get; set; }
}