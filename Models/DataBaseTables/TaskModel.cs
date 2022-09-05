using System.ComponentModel.DataAnnotations.Schema;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using VecoBackend.Enums;

namespace VecoBackend.Models;

[Table("Tasks")]
public class TaskModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }
    
    public int points { get; set; }
    
    public string description { get; set; }
    
    public TaskType type { get; set; }
    public bool isSeen { get; set; }
    public DateTime deadline { get; set; }
}