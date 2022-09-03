using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using a;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using VecoBackend.Data;
using VecoBackend.Models;
using VecoBackend.Requests;
using VecoBackend.Responses;
using VecoBackend.Services;

namespace VecoBackend.Controllers;

[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private UserService _userService;
    private JwtSettings _options;

    public UserController(UserService userService, ApplicationContext applicationContext,
        IOptions<JwtSettings> options)
    {
        _userService = userService;
        userService.AddContext(applicationContext);
        _options = options.Value;
        userService.AddJwtSettings(_options);
    }

    [HttpGet("token")]
    [AllowAnonymous]
    public string GetToken()
    {
        List<Claim> claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.Name, "admin"));
        claims.Add(new Claim(ClaimTypes.Role, "admin"));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [AllowAnonymous]
    [HttpPost("auth/signup")]
    public async Task<IActionResult> SignUp(SignUpRequest request)
    {
        var ans = await _userService.SignUp(request.name, request.password,request.email);
        if (ans.success)
            return Ok(ans.Data);
        return BadRequest(ans.Data);
    }

    [AllowAnonymous]
    [HttpPost("auth/login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        var ans = await _userService.Login(loginRequest.email, loginRequest.password);
        if (ans.success)
            return Ok(ans.Data);
        return BadRequest(ans.Data);
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUser()
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var ans = await _userService.GetUser(token);
        if (ans != null)
            return Ok(ans);
        return BadRequest("User not found");
    }

    [HttpPost("/device")]
    public async Task<IActionResult> AddDevice(AddTokenDeviceRequest request)
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var ans = await _userService.AddDevice(token, request.deviceToken);
        if (ans)
            return Ok();
        return BadRequest();
    }
    [HttpPut("edite/password")]
    public async Task<IActionResult> EditePassword(EditPasswordRequest request)
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var ans = await _userService.EditePassword(token, request.old_password, request.new_password);
        if (ans.success)
            return Ok(ans.Data);
        return BadRequest(ans.Data);
    }


    [HttpPut("/edite/username")]
    public async Task<IActionResult> EditeUsername([Bind("User")] EditUsernameRequest request)
    {
        var token = "asdf"; //Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        var ans = await _userService.EditeUsername(token, request.new_username);
        if (ans.success)
            return Ok(ans.Data);
        return BadRequest(ans.Data);
    }
}