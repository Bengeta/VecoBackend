using Microsoft.EntityFrameworkCore;
using VecoBackend.Data;
using VecoBackend.Models;

namespace VecoBackend.Services;

public class UserService
{
    private ApplicationContext context;


    public void AddContext(ApplicationContext _applicationContext)
    {
        context = _applicationContext;
    }

    public async Task<ServiseResponse<string>> Login(string username, string password)
    {
        try
        {
            var user = await context.UserModels.Where(x => x.name == username).FirstAsync();
            if (user == null)
            {
                return new ServiseResponse<string>() {success = false, Data = "User not found"};
            }

            if (user.password != password)
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


    public async Task<ServiseResponse<string>> SignUp(string name, string username, string password)
    {
        try
        {
            var user = await context.UserModels.Where(x => x.name == username).FirstAsync();
            if (user != null)
            {
                return new ServiseResponse<string>() {success = false, Data = "User already exists"};
            }
            var newUser = new UserModel()
            {
                name = name,
                username = username,
                password = password,
                token = Guid.NewGuid().ToString()
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
}