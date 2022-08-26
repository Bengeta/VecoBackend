using a;
using Microsoft.AspNetCore.Mvc;
using VecoBackend.Data;
using VecoBackend.Responses;
using VecoBackend.Services;

namespace VecoBackend.Controllers;
public class UserController : ControllerBase
{
    private UserService _userService;

    public UserController(UserService _userService, ApplicationContext _applicationContext)
    {
        _userService = _userService;
        _userService.AddContext(_applicationContext);
    } 
        [HttpPost("auth/signup")]
    public async Task<IActionResult> SignUp([Bind("User")] SignUpRequest request)
    {
        var ans = await _userService.SignUp(request.name, request.username, request.password);
        if (ans.success)
            return Ok(ans.Data);
        return BadRequest(ans.Data);
      
    }

    [HttpPost("auth/login")]
    public async Task<IActionResult> Login([Bind("User")] LoginRequest loginRequest)
    {
        var ans = await _userService.Login(loginRequest.username, loginRequest.password);
        if (ans.success)
            return Ok(ans.Data);
        return BadRequest(ans.Data);
    }

   [HttpGet("getuser")]
    public async Task<IActionResult> GetUser()
    {
        var token = "asdf";//Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var ans = await _userService.GetUser(token);
        if (ans != null)
            return Ok(ans);
        return BadRequest("User not found");
    }


    
    [HttpPut("editepassword")]
    public async Task<IActionResult> EditePassword([Bind("User")] EditPasswordRequest request)
    {
        var token = "asdf";//Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var ans = await _userService.EditePassword(token, request.old_password, request.new_password);
        if (ans.success)
            return Ok(ans.Data);
        return BadRequest(ans.Data);
    }


    [HttpPut("/editeusername")]
    public async Task<IActionResult> EditeUsername([Bind("User")] EditUsernameRequest request)
    {
        var token = "asdf";//Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var ans = await _userService.EditeUsername(token, request.new_username);
        if (ans.success)
            return Ok(ans.Data);
        return BadRequest(ans.Data);
        
    }
}