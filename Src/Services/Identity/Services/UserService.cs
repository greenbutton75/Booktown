namespace Identity.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using AutoMapper;
using Identity.Helpers;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class UserService : IUserService
{
    private readonly CognitoUserPool _pool;
    private readonly UserManager<CognitoUser> _userManager;
    private readonly SignInManager<CognitoUser> _signInManager;
    private readonly IMapper _mapper;
    private readonly AppSettings _appSettings;
    private readonly ILogger<UserService> _logger;

    public UserService(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager,
        CognitoUserPool pool, IMapper mapper, IOptions<AppSettings> appSettings, ILogger<UserService> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _pool = pool;
        _mapper = mapper;
        _appSettings = appSettings.Value;
        _logger = logger;
    }


    public async Task<UserModel?> Signup(SignupRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user != null)
        {
            _logger.LogError("User with the email {Email} already exists", model.Email);
            throw new AppException($"User with the email '{model.Email}' already exists");
        }

        user = _pool.GetUser(model.Email);

        user.Attributes.Add(CognitoAttribute.Email.AttributeName, model.Email);
        user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Username);
        var createdUser = await _userManager.CreateAsync(user, model.Password);

        // Use Lambda to auto ConfirmSignUp https://stackoverflow.com/questions/47361948/how-to-confirm-user-in-cognito-user-pools-without-verifying-email-or-phone
        // var result = await ((CognitoUserManager<CognitoUser>)_userManager).ConfirmSignUpAsync( user, model.Code, true);

        if (createdUser.Succeeded)
            return _mapper.Map<UserModel>(model);
        else
            if (createdUser.Errors.Any()) throw new AppException(createdUser.Errors.FirstOrDefault().Description);

        return null;
    }

    public async Task<AuthenticateResponse?> LogIn(LoginRequest model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.Email,   model.Password, false, false);

        if (!result.Succeeded)
        {
            _logger.LogError("User with the email {Email} not found or wrong password", model.Email);
            throw new KeyNotFoundException("User not found or wrong password");
        }

        var cognitoUser = await ((CognitoUserManager<CognitoUser>)_userManager).FindByEmailAsync(model.Email);

        var user = new UserModel
        {
            Username = cognitoUser.Attributes["name"],
            Email = model.Email
        };

        // authentication successful so generate jwt token
        var token = generateJwtToken(user);

        return new AuthenticateResponse(user, token);
    }

    // helper methods

    private string generateJwtToken(UserModel user)
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.JWTSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("name", user.Username.ToString()), new Claim("email", user.Email.ToString()) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

}