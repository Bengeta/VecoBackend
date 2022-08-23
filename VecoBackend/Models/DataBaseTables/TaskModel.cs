using System.ComponentModel.DataAnnotations.Schema;
using VecoBackend.Interfaces;

namespace VecoBackend.Models;

[Table("Tasks")]
public class TaskModel
{
    public int id { get; set; }
    
    public string name { get; set; }
    
    public string description { get; set; }
    
    public TaskType type { get; set; }
}