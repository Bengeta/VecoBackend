using Microsoft.AspNetCore.Mvc;
using VecoBackend.Interfaces;

namespace VecoBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class MigratorController : ControllerBase
{
    private readonly IMigratorService ms;

    public MigratorController(IMigratorService ms)
    {
        this.ms = ms;
    }

    [HttpGet]
    public ActionResult Version()
    {
        return Ok(new { Version = "1.00" });
    }

    /*[HttpGet("VersionInfo")]
    public ActionResult VersionInfo()
    {
        var recs = context.VersionInfo.OrderByDescending(v => v.Version);

        return Ok(recs);
    }*/

    [HttpGet("MigrateUp")]
    public ActionResult MigrateUp()
    {
        var resp = ms.MigrateUp();

        return Ok(resp);
    }

    [HttpGet("MigrateDown/{version}")]
    public ActionResult MigrateDown(long version)
    {
        var resp = ms.MigrateDown(version);

        return Ok(resp);
    }
}