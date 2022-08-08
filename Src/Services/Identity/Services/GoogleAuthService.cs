using Identity.External.Contracts;
using Identity.Helpers;
using Identity.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Identity.Services;

public class GoogleAuthService : IGoogleAuthService
{
    private const string UserInfoUrl = "https://www.googleapis.com/oauth2/v4/token";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GoogleAuthSettings _googleAuthSettings;

    public GoogleAuthService(FacebookAuthSettings facebookAuthSettings, GoogleAuthSettings googleAuthSettings, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _googleAuthSettings = googleAuthSettings;
    }
    public async Task<GoogleUserInfoResult?> GetUserInfoAsync(string accessToken)
    {
        try
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", _googleAuthSettings.ClientId),
                new KeyValuePair<string, string>("client_secret", _googleAuthSettings.ClientSecret),
                new KeyValuePair<string, string>("code", accessToken),
                new KeyValuePair<string, string>("redirect_uri", _googleAuthSettings.RedirectUrl)
            });


                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var response = await httpClient.PostAsync(UserInfoUrl, content);


                string result = response.Content.ReadAsStringAsync().Result;

                var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(result);

                var id_token = dict["id_token"].ToString();
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadToken(id_token) as JwtSecurityToken;

                return new GoogleUserInfoResult { Name = token.Claims.First(claim => claim.Type == "name").Value, Email = token.Claims.First(claim => claim.Type == "email").Value };
            }
        }
        catch (Exception)
        {
            throw new AppException("GoogleAuth failed"); ;
        }

    }
}