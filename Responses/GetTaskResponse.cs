using VecoBackend.Enums;
using TaskStatus = VecoBackend.Enums.TaskStatus;

namespace VecoBackend.Responses;

public class GetTaskResponse
{
    public int Id { get; set; }

    public int Points { get; set; }
    public string Title { get; set; }

    public string Description { get; set; }
    public TaskStatus Status { get; set; }
    public TaskType Type { get; set; }
    public bool IsSeen { get; set; }
    public long Deadline { get; set; }
}