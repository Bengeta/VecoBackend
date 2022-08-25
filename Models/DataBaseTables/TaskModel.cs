using System.ComponentModel.DataAnnotations.Schema;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using VecoBackend.Interfaces;

namespace VecoBackend.Models;

[Table("Tasks")]
public class TaskModel
{
    public int id { get; set; }
    
    public int points { get; set; }
    
    public string description { get; set; }
    
    public TaskType type { get; set; }
    public bool isSeen { get; set; }
}