namespace VecoBackend.Responses;

public class EditPasswordResponse
{
    public string token { get; set; }
    public string old_password { get; set; }
    public string new_password { get; set; }
}