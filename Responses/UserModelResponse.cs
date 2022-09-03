namespace VecoBackend.Responses;

public class UserModelResponse
{
    public int id { get; set; }
    public string username { get; set; }

    public string name { get; set; }

    public Boolean isAdmin { get; set; }

    public int points { get; set; }

    public string email { get; set; }
}