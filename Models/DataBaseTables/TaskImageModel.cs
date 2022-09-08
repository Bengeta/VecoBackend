using System.ComponentModel.DataAnnotations.Schema;

namespace VecoBackend.Models;
[Table("TaskImage")]
public class TaskImageModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }
    public int UserTaskId { get; set; }
    public int imageId { get; set; }
}