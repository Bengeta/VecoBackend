using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskStatus = VecoBackend.Enums.TaskStatus;

namespace VecoBackend.Models;

[Table("UserTasks")]
public class UserTaskModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }
    [ForeignKey("UserModel")]
    public int user_id { get; set; }

    [ForeignKey("TaskModel")]
    public int task_id { get; set; }
    public UserModel user { get; set; }
    public TaskModel task { get; set; }
    public TaskStatus task_status { get; set; }
    public List<TaskImageModel> task_images { get; set; }
}