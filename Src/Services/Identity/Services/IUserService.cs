namespace Identity.Services;

using Identity.Models;
using System.Threading.Tasks;

public interface IUserService
{
    public Task<UserModel?> Signup(SignupRequest model);
    public Task<UserModel?> LogIn(LoginRequest model);

    public Task<UserModel?> LogInWithFacebook(string token);

}

