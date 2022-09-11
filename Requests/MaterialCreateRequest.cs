using VecoBackend.Enums;

namespace VecoBackend.Requests;

public class MaterialCreateRequest
{
    public string Author { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public bool IsSeen { get; set; }
    public MaterialCategory Category { get; set; }
}