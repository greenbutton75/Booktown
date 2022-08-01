namespace Identity.Services;

using Identity.Models;
using System.Threading.Tasks;

public interface IUserService
{
    public Task<UserModel?> Signup(SignupRequest model);
    public Task<AuthenticateResponse?> LogIn(LoginRequest model);
    public string generateJwtToken(UserModel user);
}

