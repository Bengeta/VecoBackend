using VecoBackend.Enums;

namespace VecoBackend.Models;

public class ResponseModel<T>
{
    public ResultCode ResultCode { get; set; }
    public T? Data { get; set; }
}