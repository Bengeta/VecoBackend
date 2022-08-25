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


/*    [HttpPost("/getuser")]
    public async Task<ResponseModel<UserModel>> GetUser([Bind("User")] TokenResponse response)
    {
        var ans = await _userService.GetUser(response.token);
        var answer = new ResponseModel<UserModel>();
        answer.success = false;
        if (ans != null)
        {
            answer.data = ans;
            answer.success = true;
        }

        return answer;
    }
    */


    /*
    [HttpPut("/editepassword")]
    public async Task<ResponseModel<String>?> EditePassword([Bind("User")] EditPasswordResponse response)
    {
        var ans = await _userService.ChangePassword(response.token, response.old_password, response.new_password);
        var answer = new ResponseModel<String>();
        answer.success = false;
        if (ans != null)
        {
            answer.data = ans;
            answer.success = true;
        }

        return answer;
    }


    [HttpPut("/editeusername")]
    public async Task<Result> EditeUsername([Bind("User")] EditUsernameResponse response)
    {
        return await _userService.ChangeUsername(response.token, response.name);
    }

    [HttpPost("/lastlogins")]
    public async Task<ResponseModel<List<HistoryLoginModel>>> GetUserLoginHistory([Bind("User")] TokenResponse response)
    {
        var ans = await _userService.GetLoginHistory(response.token);
        var answer = new ResponseModel<List<HistoryLoginModel>>();
        answer.success = false;
        if (ans.Count > 0)
        {
            answer.data = ans;
            answer.success = true;
        }

        return answer;
    }*/
}