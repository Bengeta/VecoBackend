using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskStatus = VecoBackend.Interfaces.TaskStatus;

namespace VecoBackend.Models;

[Table("UserTasks")]
public class UserTaskModel
{
    public int user_id { get; set; }
    public int task_id { get; set; }
    public TaskStatus task_status { get; set; }
    public string photos { get; set; }
}