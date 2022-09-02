namespace VecoBackend.Responses;

public class CheckTaskListResponse
{
    public int userTaskId { get; set; }
    public string description { get; set; }
    public string UserName { get; set; }
    public int points { get; set; }
}