namespace VecoBackend.Services;

public class ServiseResponse<T>
{
    public Boolean success { get; set; }
    public T Data { get; set; }
}