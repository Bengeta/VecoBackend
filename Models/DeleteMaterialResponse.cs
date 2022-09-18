namespace VecoBackend.Models;

public class DeleteMaterialResponse
{
    public int materialId { get; set; }
    public int? imageId { get; set; }
    public string? imagePath { get; set; }
    public int? materialImageId { get; set; }
}