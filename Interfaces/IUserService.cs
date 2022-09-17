using VecoBackend.Models;
using VecoBackend.Responses;

namespace VecoBackend.Interfaces;

public interface IUserService
{
    public Task<ResponseModel<string>> Login(string email, string password);
    public Task<ResponseModel<string>> SignUp(string name, string password, string email);
    public Task<ResponseModel<UserModelResponse>> GetUser(string token);
    public Task<ResponseModel<string>> EditePassword(string token, string oldPassword, string newPassword);
    public Task<ResponseModel<string>> EditeUsername(string token, string newUsername);
    public Task<ResponseModel<string>> AddDevice(string token, string deviceToken);
}