namespace Identity.Services;

using Identity.Models;
using System.Threading.Tasks;

public interface ITokenService
{
    public Task<TokenModel?> GetRefreshToken(TokenModel tokenModel);

    public Task RevokeRefreshTokens(string token);

    public Task<TokenModel> GenerateJwtTokens(UserModel user);

    public UserModel ValidateJwtToken(string token);
}

