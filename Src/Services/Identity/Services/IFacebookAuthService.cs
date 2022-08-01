namespace Identity.Services;

using System.Threading.Tasks;
using Identity.External.Contracts;

public interface IFacebookAuthService
{
    Task<FacebookTokenValidationResult?> ValidateAccessTokenAsync(string accessToken);
    Task<FacebookUserInfoResult?> GetUserInfoAsync(string accessToken);
}

