using VecoBackend.Services;
using VecoBackend.Enums;
namespace VecoBackend.Responses;

public class ChangeTaskStatusResponse
{
    public Enums.TaskStatus newStatus { get; set; }
    public int taskId { get; set; }
}