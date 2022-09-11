using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VecoBackend.Models;
[Table("ImageStorage")]
public class ImageStorageModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }
    [ForeignKey("UserModel")]
    public int userId { get; set; }
    public string imagePath { get; set; }
    public bool isUsed { get; set; }
    public UserModel UserModel { get; set; }
    public List<TaskImageModel> TaskImageModel { get; set; }
}