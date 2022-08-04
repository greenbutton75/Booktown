namespace Identity.Services;

using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Identity.Helpers;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class TokenService : ITokenService
{
    private readonly AppSettings _appSettings;
    private readonly ILogger<TokenService> _logger;
    private readonly ConcurrentDictionary<string, RefreshTokenInfo> refreshTokens = new ConcurrentDictionary<string, RefreshTokenInfo>();

    public TokenService(IOptions<AppSettings> appSettings, ILogger<TokenService> logger)
    {
        _appSettings = appSettings.Value;
        _logger = logger;
    }


    public async Task<TokenModel> GenerateJwtTokens(UserModel user)
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.JWTSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("name", user.Username.ToString()), new Claim("email", user.Email.ToString()) }),
            Expires = DateTime.UtcNow.AddMinutes(_appSettings.TokenValidityInMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var refreshToken = await CreateRefreshToken(user);
        return new TokenModel { Token = tokenHandler.WriteToken(token) , RefreshToken= refreshToken };
    }

    public async Task<TokenModel?> GetRefreshToken(TokenModel tokenModel)
    {
        // Current token can be expired
        JwtSecurityToken jwtToken = CheckToken(tokenModel.Token, false);

        var userEmail = jwtToken.Claims.First(x => x.Type == "email").Value;
        var userName = jwtToken.Claims.First(x => x.Type == "name").Value;

        // Check refreshToken 
        var checkResult = CheckRefreshToken(userEmail, tokenModel.RefreshToken);

        if (checkResult == CheckRefreshTokenResult.Ok)
        {
            refreshTokens[tokenModel.RefreshToken].IsUsed = true;

            return await GenerateJwtTokens(new UserModel
            {
                Username = userName,
                Email = userEmail
            });
        }

        // If refreshToken is already used - InvalidateUserRefreshTokens
        if (checkResult == CheckRefreshTokenResult.IsUsed)
            InvalidateUserRefreshTokens(userEmail);


        throw new AppException($"Refresh Token is invalid {checkResult.ToString ()}");

    }

    public Task RevokeRefreshTokens(string token)
    {
        JwtSecurityToken jwtToken = CheckToken(token, true);
        var userEmail = jwtToken.Claims.First(x => x.Type == "email").Value;

        InvalidateUserRefreshTokens(userEmail);

        return Task.CompletedTask;
    }


    public UserModel ValidateJwtToken(string token)
    {
        JwtSecurityToken jwtToken = CheckToken(token, true);

        var userEmail = jwtToken.Claims.First(x => x.Type == "email").Value;
        var userName = jwtToken.Claims.First(x => x.Type == "name").Value;

        return new UserModel
        {
            Username = userName,
            Email = userEmail
        };
    }
    private void InvalidateUserRefreshTokens(string userEmail)
    {
        // Invalidate all user refreshTokens
        foreach (var item in refreshTokens)
        {
            if (item.Value.Email == userEmail && !item.Value.IsUsed)
                item.Value.IsUsed = true;
        }
    }

    private JwtSecurityToken CheckToken(string token, bool validateLifetime)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.JWTSecret);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = validateLifetime,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            return (JwtSecurityToken)validatedToken;
        }
        catch (Exception ex)
        {
            throw new AppException ($"Token is invalid {ex.Message}");
        }

    }

    private CheckRefreshTokenResult CheckRefreshToken(string userEmail, string refreshToken)
    {
        if (!refreshTokens.ContainsKey(refreshToken)) return CheckRefreshTokenResult.NotFound;

        if (refreshTokens[refreshToken].Email!= userEmail) return CheckRefreshTokenResult.Incorrect;
        if (refreshTokens[refreshToken].IsUsed ) return CheckRefreshTokenResult.IsUsed;
        if (refreshTokens[refreshToken].Expiration < DateTime.UtcNow) return CheckRefreshTokenResult.Expired;

        return CheckRefreshTokenResult.Ok;
    }
    private Task<string> CreateRefreshToken(UserModel user)
    {
        var id = Guid.NewGuid().ToString ();
        refreshTokens.TryAdd(id, new RefreshTokenInfo {Email=user.Email, IsUsed=false, Expiration= DateTime.UtcNow .AddDays (_appSettings.RefreshTokenValidityInDays) });

        return Task.FromResult(id);
    }

    private class RefreshTokenInfo
    {
        public string? Email { get; set; }
        public bool IsUsed { get; set; }
        public DateTime Expiration { get; set; }
    }

    enum CheckRefreshTokenResult
    {
        Ok,
        NotFound,
        Expired,
        IsUsed,
        Incorrect
    }
}