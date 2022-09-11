using System.ComponentModel.DataAnnotations.Schema;

namespace VecoBackend.Models;

public class MaterialImageModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [ForeignKey("MaterialModel")]
    public int MaterialId { get; set; }
    [ForeignKey("ImageStorageModel")]
    public int ImageId { get; set; }
    public MaterialModel MaterialModel { get; set; }
    public ImageStorageModel ImageStorageModel { get; set; }
}
