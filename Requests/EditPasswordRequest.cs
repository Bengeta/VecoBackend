namespace VecoBackend.Responses;

public class EditPasswordRequest
{
    public string old_password { get; set; }
    public string new_password { get; set; }
}