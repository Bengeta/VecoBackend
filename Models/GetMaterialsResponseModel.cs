using VecoBackend.Enums;

namespace VecoBackend.Models;

public class GetMaterialsResponseModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Author { get; set; }
    public DateTime Date { get; set; }
    public string? imagePath { get; set; }
    public MaterialCategory Category { get; set; }
    public ImageType? imageType { get; set; }
}