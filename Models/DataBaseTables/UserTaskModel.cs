using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskStatus = VecoBackend.Enums.TaskStatus;

namespace VecoBackend.Models;

[Table("UserTasks")]
public class UserTaskModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }
    public int user_id { get; set; }
    public int task_id { get; set; }
    public TaskStatus task_status { get; set; }
}