using System.ComponentModel.DataAnnotations.Schema;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using VecoBackend.Enums;
using TaskStatus = VecoBackend.Enums.TaskStatus;

namespace VecoBackend.Models;

[Table("Tasks")]
public class TaskModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public int Points { get; set; }
    public string Title { get; set; }
    
    public string Description { get; set; }
    [NotMapped]
    public TaskStatus Status { get; set; }
    public TaskType Type { get; set; }
    public bool IsSeen { get; set; }
    public DateTime Deadline { get; set; }
    public List<UserTaskModel> userTasks { get; set; }
}