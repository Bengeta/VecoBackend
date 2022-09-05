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
    public async Task<ResponseModel<string>> SignUp(SignUpRequest request)
    {
        return await _userService.SignUp(request.name, request.password, request.email);
    }

    [AllowAnonymous]
    [HttpPost("auth/login")]
    public async Task<ResponseModel<string>> Login(LoginRequest loginRequest)
    {
        return await _userService.Login(loginRequest.email, loginRequest.password);
    }

    [HttpGet("user")]
    public async Task<ResponseModel<UserModelResponse>> GetUser()
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        return await _userService.GetUser(token);
    }

    [HttpPost("/device")]
    public async Task<ResponseModel<string>> AddDevice(AddTokenDeviceRequest request)
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        return await _userService.AddDevice(token, request.DeviceToken);
    }

    [HttpPut("edite/password")]
    public async Task<ResponseModel<string>> EditePassword(EditPasswordRequest request)
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        return await _userService.EditePassword(token, request.old_password, request.new_password);
    }


    [HttpPut("/edite/username")]
    public async Task<ResponseModel<string>> EditeUsername([Bind("User")] EditUsernameRequest request)
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        return await _userService.EditeUsername(token, request.new_username);
    }
}