namespace Identity.Models;

public class AuthenticateResponse
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }


    public AuthenticateResponse(UserModel user, string token, string refreshToken)
    {
        Username = user.Username;
        Email = user.Email;
        Token = token;
        RefreshToken = refreshToken; 
    }
}