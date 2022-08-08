using Identity.External.Contracts;
using Identity.Helpers;
using Identity.Options;
using System.Text.Json;

namespace Identity.Services;

public class FacebookAuthService : IFacebookAuthService
{
    private const string TokenValidationUrl = "https://graph.facebook.com/debug_token?access_token={0}|{1}&input_token={2}";
    private const string UserInfoUrl = "https://graph.facebook.com/me?fields=first_name,last_name,email&access_token={0}";
    private readonly FacebookAuthSettings _facebookAuthSettings;
    private readonly IHttpClientFactory _httpClientFactory;

    public FacebookAuthService(FacebookAuthSettings facebookAuthSettings, IHttpClientFactory httpClientFactory)
    {
        _facebookAuthSettings = facebookAuthSettings;
        _httpClientFactory = httpClientFactory;
    }
    public async Task<FacebookTokenValidationResult?> ValidateAccessTokenAsync(string accessToken)
    {
        try
        {
            var formattedUrl = string.Format(TokenValidationUrl, _facebookAuthSettings.AppId, _facebookAuthSettings.AppSecret, accessToken);

            var result = await _httpClientFactory.CreateClient().GetAsync(formattedUrl);
            result.EnsureSuccessStatusCode();

            var responseAsString = await result.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<FacebookTokenValidationResult>(responseAsString);
        }
        catch (Exception)
        {
            throw new AppException("FacebookAuth failed"); ;
        }
    }
    public async Task<FacebookUserInfoResult?> GetUserInfoAsync(string accessToken)
    {
        try
        {
            var formattedUrl = string.Format(UserInfoUrl, accessToken);

            var result = await _httpClientFactory.CreateClient().GetAsync(formattedUrl);
            result.EnsureSuccessStatusCode();

            var responseAsString = await result.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<FacebookUserInfoResult>(responseAsString);
        }
        catch (Exception)
        {
            throw new AppException("FacebookAuth failed"); ;
        }
    }
}