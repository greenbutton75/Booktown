namespace Identity.Helpers;

public class AppSettings
{
    public string JWTSecret { get; set; }
    public int TokenValidityInMinutes { get; set; }
    public int RefreshTokenValidityInDays { get; set; }
}