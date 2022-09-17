using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VecoBackend.Data;
using VecoBackend.Enums;
using VecoBackend.Interfaces;
using VecoBackend.Models;
using VecoBackend.Responses;
using VecoBackend.Utils;

namespace VecoBackend.Services;

public class UserService:IUserService
{
    private ApplicationContext context;
    private JwtSettings _options;

    public UserService(ApplicationContext context,IOptions<JwtSettings> options)
    {
        this.context = context;
        _options = options.Value;
    }
    public void AddJwtSettings(JwtSettings _options)
    {
        this._options = _options;
    }

    public async Task<ResponseModel<string>> Login(string email, string password)
    {
        try
        {
            var user = await context.UserModels.Where(x => x.email == email).FirstOrDefaultAsync();
            if (user == null)
                return new ResponseModel<string>() {ResultCode = ResultCode.UserNotFound};

            var hashedPassword = Hash.GenerateHashFromSalt(password, user.salt);
            if (hashedPassword != user.password)
                return new ResponseModel<string>() {ResultCode = ResultCode.PasswordIncorrect};

            return new ResponseModel<string>() {ResultCode = ResultCode.Success, Data = user.token};
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ResponseModel<string>() {ResultCode = ResultCode.Failed};
        }
    }


    public async Task<ResponseModel<string>> SignUp(string name, string password, string email)
    {
        try
        {
            var user = await context.UserModels.Where(x => x.email == email).AnyAsync();
            if (user)
                return new ResponseModel<string>() {ResultCode = ResultCode.UserAlreadyExists};

            var hashAndSalt = Hash.GenerateHash(password);
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, name));
            claims.Add(new Claim(ClaimTypes.Email, email));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );

            var Token = new JwtSecurityTokenHandler().WriteToken(token);
            var newUser = new UserModel()
            {
                name = name,
                password = hashAndSalt.Key,
                salt = hashAndSalt.Value,
                token = new JwtSecurityTokenHandler().WriteToken(token),
                email = email
            };
            context.UserModels.Add(newUser);
            await context.SaveChangesAsync();
            return new ResponseModel<string>() {ResultCode = ResultCode.Success, Data = newUser.token};
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ResponseModel<string>() {ResultCode = ResultCode.Failed};
        }
    }

    public async Task<ResponseModel<UserModelResponse>> GetUser(string token)
    {
        try
        {
            var user = await context.UserModels.Where(x => x.token == token).FirstOrDefaultAsync();
            if (user == null)
                return null;
            var ans = new UserModelResponse()
            {
                email = user.email,
                name = user.name,
                id = user.id,
                points = user.points,
                isAdmin = user.isAdmin
            };

            return new ResponseModel<UserModelResponse>() {ResultCode = ResultCode.Success, Data = ans};
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ResponseModel<UserModelResponse>() {ResultCode = ResultCode.Failed};
        }
    }

    public async Task<ResponseModel<string>> EditePassword(string token, string oldPassword, string newPassword)
    {
        try
        {
            var user = await context.UserModels.Where(x => x.token == token).FirstOrDefaultAsync();
            if (user == null)
                return new ResponseModel<string>() {ResultCode = ResultCode.UserNotFound};

            var password = Hash.GenerateHashFromSalt(oldPassword, user.salt);
            if (user.password != password)
                return new ResponseModel<string>() {ResultCode = ResultCode.PasswordIncorrect};

            user.password = Hash.GenerateHashFromSalt(newPassword, user.salt);
            await context.SaveChangesAsync();
            return new ResponseModel<string>() {ResultCode = ResultCode.Success};
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ResponseModel<string>() {ResultCode = ResultCode.Failed};
        }
    }

    public async Task<ResponseModel<string>> EditeUsername(string token, string newUsername)
    {
        try
        {
            var user = await context.UserModels.Where(x => x.token == token).FirstOrDefaultAsync();
            if (user == null)
                return new ResponseModel<string>() {ResultCode = ResultCode.UserNotFound};

            user.name = newUsername;
            await context.SaveChangesAsync();
            return new ResponseModel<string>() {ResultCode = ResultCode.Success};
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ResponseModel<string>() {ResultCode = ResultCode.Failed};
        }
    }

    public async Task<ResponseModel<string>> AddDevice(string token, string deviceToken)
    {
        try
        {
            var user = await context.UserModels.Where(x => x.token == token).FirstOrDefaultAsync();
            if (user == null)
                return new ResponseModel<string>() {ResultCode = ResultCode.UserNotFound};

            var notification = new NotificationTokensModel()
            {
                Token = deviceToken,
                UserId = user.id
            };
            context.NotificationTokensModels.Add(notification);
            context.SaveChanges();
            return new ResponseModel<string>() {ResultCode = ResultCode.Success};
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ResponseModel<string>() {ResultCode = ResultCode.Failed};
        }
    }
}