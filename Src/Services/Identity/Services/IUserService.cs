namespace Identity.Services;

using Identity.Models;
using System.Threading.Tasks;

public interface IUserService
{
    public Task<UserModel?> SignupAsync(SignupRequest model);
    public Task<UserModel?> LogInAsync(LoginRequest model);

    public Task<UserModel?> LogInWithFacebookAsync(string token);
    public Task<UserModel?> LogInWithGoogleAsync(string token);

}

