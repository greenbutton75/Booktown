namespace Identity.Services;

using System.Threading.Tasks;
using Identity.External.Contracts;

public interface IGoogleAuthService
{
    Task<GoogleUserInfoResult?> GetUserInfoAsync(string accessToken);
}

