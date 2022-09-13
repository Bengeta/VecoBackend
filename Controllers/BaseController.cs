using Microsoft.AspNetCore.Mvc;

namespace VecoBackend.Controllers;

public class BaseController:ControllerBase
{
    protected string Token() => HttpContext.Items["Token"].ToString();
    protected string BaseUrl() => $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
}