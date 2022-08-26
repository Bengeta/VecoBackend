using VecoBackend.Services;
using VecoBackend.Enums;
namespace VecoBackend.Responses;

public class ChangeTaskStatusRequest
{
    public Enums.TaskStatus newStatus { get; set; }
    public int taskId { get; set; }
}