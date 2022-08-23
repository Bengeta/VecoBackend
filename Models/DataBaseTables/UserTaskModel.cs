using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VecoBackend.Models;

[Table("UserTasks")]
public class UserTaskModel
{
    public int user_id { get; set; }
    public UserModel user { get; set; }
    public TaskModel task { get; set; }
    public int task_id { get; set; }
    public int task_status { get; set; }
    public string photos { get; set; }
}