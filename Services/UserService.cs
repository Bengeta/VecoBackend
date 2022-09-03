using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VecoBackend.Data;
using VecoBackend.Models;
using VecoBackend.Responses;

namespace VecoBackend.Services;

public class UserService
{
    private ApplicationContext context;
    private JwtSettings _options;


    public void AddContext(ApplicationContext applicationContext)
    {
        context = applicationContext;
    }

    public void AddJwtSettings(JwtSettings _options)
    {
        this._options = _options;
    }

    public async Task<ServiseResponse<string>> Login(string username, string password)
    {
        try
        {
            var user = await context.UserModels.Where(x => x.username == username).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ServiseResponse<string>() {success = false, Data = "User not found"};
            }

            var hashedPassword = GenerateHashFromSalt(password, user.salt);
            if (hashedPassword != user.password)
            {
                return new ServiseResponse<string>() {success = false, Data = "Password is incorrect"};
            }

            return new ServiseResponse<string>() {success = true, Data = user.token};
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ServiseResponse<string>() {success = false, Data = "Something went wrong"};
        }
    }


    public async Task<ServiseResponse<string>> SignUp(string name, string username, string password,string email)
    {
        try
        {
            var user = await context.UserModels.Where(x => x.name == username).AnyAsync();
            if (user)
            {
                return new ServiseResponse<string>() {success = false, Data = "User already exists"};
            }
            var hashAndSalt = GenerateHash(password);

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, name));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, username));
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
                username = username,
                password = hashAndSalt.Key,
                salt = hashAndSalt.Value,
                token = new JwtSecurityTokenHandler().WriteToken(token),
                email = email
            };
            context.UserModels.Add(newUser);
            await context.SaveChangesAsync();
            return new ServiseResponse<string>() {success = true, Data = newUser.token};
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ServiseResponse<string>() {success = false, Data = "Something went wrong"};
        }
    }

    public async Task<UserModelResponse> GetUser(string token)
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
                username = user.username,
                id = user.id,
                points = user.points,
                isAdmin = user.isAdmin
            };

            return ans;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<ServiseResponse<string>> EditePassword(string token, string old_password, string new_password)
    {
        try
        {
            var user = await context.UserModels.Where(x => x.token == token).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ServiseResponse<string>() {success = false, Data = "User not found"};
            }
            var password = GenerateHashFromSalt(old_password, user.salt);
            if (user.password != password)
            {
                return new ServiseResponse<string>() {success = false, Data = "Old password is incorrect"};
            }

            user.password = GenerateHashFromSalt(new_password, user.salt);
            await context.SaveChangesAsync();
            return new ServiseResponse<string>() {success = true, Data = "Password changed"};
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ServiseResponse<string>() {success = false, Data = "Something went wrong"};
        }
    }

    public async Task<ServiseResponse<string>> EditeUsername(string token, string newUsername)
    {
        try
        {
            var user = await context.UserModels.Where(x => x.token == token).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ServiseResponse<string>() {success = false, Data = "User not found"};
            }

            user.username = newUsername;
            await context.SaveChangesAsync();
            return new ServiseResponse<string>() {success = true, Data = "Username changed"};
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ServiseResponse<string>() {success = false, Data = "Something went wrong"};
        }
    }

    public async Task<Boolean> AddDevice(string token, string deviceToken)
    {
        try
        {
            var user = await context.UserModels.Where(x => x.token == token).FirstOrDefaultAsync();
            if (user == null)
            {
                return false;
            }

            var notification = new NotificationTokensModel()
            {
                Token = deviceToken,
                UserId = user.id
            };
            context.NotificationTokensModels.Add(notification);
            context.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
    
    
    private KeyValuePair<string, string> GenerateHash(string s)
    {
        var salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        var stringSalt = Convert.ToBase64String(salt);
        var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(s, salt, KeyDerivationPrf.HMACSHA1, 1000, 256 / 8));
        return new KeyValuePair<string, string>(hash, stringSalt);
    }

    private static string GenerateHashFromSalt(string s, string strSalt) => Convert.ToBase64String(
        KeyDerivation.Pbkdf2(s, Convert.FromBase64String(strSalt), KeyDerivationPrf.HMACSHA1, 1000, 256 / 8));
}