using System.ComponentModel.DataAnnotations.Schema;

namespace VecoBackend.Models;
[Table("TaskPhoto")]
public class TaskPhotoModel
{
    public int id { get; set; }
    public int UserTaskId { get; set; }
    public string photoPath { get; set; }
}