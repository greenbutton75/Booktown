using Identity.External.Contracts;
using Identity.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Identity.Services;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GoogleAuthSettings _googleAuthSettings;

    public GoogleAuthService(FacebookAuthSettings facebookAuthSettings, GoogleAuthSettings googleAuthSettings, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _googleAuthSettings = googleAuthSettings;
    }
    public async Task<GoogleUserInfoResult?> GetUserInfoAsync(string accessToken)
    {
        using (var httpClient = new HttpClient())
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", _googleAuthSettings.ClientId),
                new KeyValuePair<string, string>("client_secret", _googleAuthSettings.ClientSecret),
                new KeyValuePair<string, string>("code", accessToken),
                new KeyValuePair<string, string>("redirect_uri", "https://pkoin.net")
            });


            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await httpClient.PostAsync("https://www.googleapis.com/oauth2/v4/token", content);


            string result = response.Content.ReadAsStringAsync().Result;

            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(result);

            var id_token = dict["id_token"].ToString ();
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadToken(id_token) as JwtSecurityToken;

            return new GoogleUserInfoResult { Name = token.Claims.First(claim => claim.Type == "name").Value, Email = token.Claims.First(claim => claim.Type == "email").Value };
        }


        //var result = await _httpClientFactory.CreateClient().GetAsync(formattedUrl);
       // result.EnsureSuccessStatusCode();
/*
        var responseAsString = await result.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<GoogleUserInfoResult>(responseAsString);
*/
/*
        var stream = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImZkYTEwNjY0NTNkYzlkYzNkZDkzM2E0MWVhNTdkYTNlZjI0MmIwZjciLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiI5ODQzNjIzNDc2MDUtZmRtM3Z0YThjY2swOTRybWQxM2Q2MW0xaDA5cmYwZTYuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJhdWQiOiI5ODQzNjIzNDc2MDUtZmRtM3Z0YThjY2swOTRybWQxM2Q2MW0xaDA5cmYwZTYuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJzdWIiOiIxMDI3NDYyNjQ4MDY3ODkzNzMzMDMiLCJlbWFpbCI6ImdyZWVuYnV0dG9uNzVAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsImF0X2hhc2giOiI1M3NHLW55LWFKZUJ3SE9DMG8zVGF3IiwibmFtZSI6IlZhbGVudGluIEtvbGVzb3YiLCJwaWN0dXJlIjoiaHR0cHM6Ly9saDMuZ29vZ2xldXNlcmNvbnRlbnQuY29tL2EvQUl0YnZta3ZDMkVXdjhVbHRqd2pUOEZBb0tRSTVkckUwTDZ2ODAxUF82b0o9czk2LWMiLCJnaXZlbl9uYW1lIjoiVmFsZW50aW4iLCJmYW1pbHlfbmFtZSI6IktvbGVzb3YiLCJsb2NhbGUiOiJydSIsImlhdCI6MTY1OTcwNzcyMSwiZXhwIjoxNjU5NzExMzIxfQ.U1lGnbhnYZMQySyuwTYEbc56psBe2LV9lQ7kMXxTwSQJIyLZz8UrLzXEyQYM_wSEgsfl3j8rElV-W7-h1POnQ58p7bhPChuBoI22baLDT7n5lrNTggIfjtOzLwUNAf7vrl9p9t_p1gzrixoizxM2bAbDb6wJDkrbV2XMY8QFeQCds7w5wf8Ubrz5fdI9R1uPrfPakYcsC2LCkfGY6zUjreFJj63vAy3uGsk29waSzwn8N5EcQs15oTBKkFvBcyBUrR6_IC8jySk1v2u2arkSHHqRGamoLyWcB5QPmleQmeZWNzqbJdtm_HnECkj5STGMcQ_gZ69aoQjDGxJSDEdmMg";
        var handler = new JwtSecurityTokenHandler();
        var token1 = handler.ReadToken(stream) as JwtSecurityToken;
        var token2 = token1 as JwtSecurityToken;

        return Ok();*/

    }
}