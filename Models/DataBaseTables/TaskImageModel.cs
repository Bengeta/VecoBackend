using System.ComponentModel.DataAnnotations.Schema;

namespace VecoBackend.Models;
[Table("TaskImage")]
public class TaskImageModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }
    [ForeignKey("UserTaskModel")]
    public int UserTaskId { get; set; }
    [ForeignKey("ImageStorageModel")]
    public int imageId { get; set; }
    public UserTaskModel UserTask { get; set; }
    public ImageStorageModel ImageStorage { get; set; }
}