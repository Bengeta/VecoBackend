using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VecoBackend.Models;
[Table("ImageStorage")]
public class ImageStorageModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }
    public int userId { get; set; }
    public string imagePath { get; set; }
}