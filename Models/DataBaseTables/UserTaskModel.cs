using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskStatus = VecoBackend.Enums.TaskStatus;

namespace VecoBackend.Models;

[Table("UserTasks")]
public class UserTaskModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [ForeignKey("UserModel")]
    public int UserId { get; set; }

    [ForeignKey("TaskModel")]
    public int TaskId { get; set; }
    public DateTime DeleteTime { get; set; }
    public UserModel user { get; set; }
    public TaskModel task { get; set; }
    public TaskStatus taskStatus { get; set; }
    public List<TaskImageModel> task_images { get; set; }
}