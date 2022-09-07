using System.ComponentModel.DataAnnotations.Schema;

namespace VecoBackend.Models;
[Table("PhotoBuffer")]
public class PhotoBufferModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }
    public int userId { get; set; }
    public string photoPath { get; set; }
}