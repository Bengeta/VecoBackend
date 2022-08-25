using a;
using Microsoft.AspNetCore.Mvc;
using VecoBackend.Data;
using VecoBackend.Responses;
using VecoBackend.Services;

namespace VecoBackend.Controllers;
[Route("user")]
public class UserController : ControllerBase
{
    private UserService _userService;

    public UserController(UserService _userService, ApplicationContext _applicationContext)
    {
        _userService = _userService;
        _userService.AddContext(_applicationContext);
    } 
        [HttpPost("/signup")]
    public async Task<IActionResult> SignUp([Bind("User")] SignUpResponse response)
    {
        var ans = await _userService.SignUp(response.name, response.username, response.password);
        if (ans.success)
            return Ok(ans.Data);
        return BadRequest(ans.Data);
      
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login([Bind("User")] LoginRequest loginRequest)
    {
        var ans = await _userService.Login(loginRequest.username, loginRequest.password);
        if (ans.success)
            return Ok(ans.Data);
        return BadRequest(ans.Data);
    }

   [HttpGet("/getuser")]
    public async Task<IActionResult> GetUser()
    {
        var token = "asdf";//Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var ans = await _userService.GetUser(token);
        if (ans != null)
            return Ok(ans);
        return BadRequest("User not found");
    }


    
    [HttpPut("/editepassword")]
    public async Task<IActionResult> EditePassword([Bind("User")] EditPasswordResponse response)
    {
        var token = "asdf";//Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var ans = await _userService.EditePassword(token, response.old_password, response.new_password);
        if (ans.success)
            return Ok(ans.Data);
        return BadRequest(ans.Data);
    }


    [HttpPut("/editeusername")]
    public async Task<IActionResult> EditeUsername([Bind("User")] EditUsernameResponse response)
    {
        var token = "asdf";//Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var ans = await _userService.EditeUsername(token, response.new_username);
        if (ans.success)
            return Ok(ans.Data);
        return BadRequest(ans.Data);
        
    }
}