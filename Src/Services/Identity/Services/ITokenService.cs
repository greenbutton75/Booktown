namespace Identity.Services;

using Identity.Models;
using System.Threading.Tasks;

public interface ITokenService
{
    public Task<TokenModel?> GetRefreshTokenAsync(TokenModel tokenModel);

    public Task RevokeRefreshTokensAsync(string token);

    public Task<TokenModel> GenerateJwtTokensAsync(UserModel user);

    public UserModel ValidateJwtToken(string token);
}

