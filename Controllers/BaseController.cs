using Microsoft.AspNetCore.Mvc;

namespace VecoBackend.Controllers;

public class BaseController:ControllerBase
{
    protected string Token() => HttpContext.Items["Token"].ToString();
}