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
    private readonly IFacebookAuthService _facebookAuthService;

    public UserService(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager,
        CognitoUserPool pool, IMapper mapper, IOptions<AppSettings> appSettings, ILogger<UserService> logger, IFacebookAuthService facebookAuthService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _pool = pool;
        _mapper = mapper;
        _appSettings = appSettings.Value;
        _facebookAuthService = facebookAuthService;
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

    public async Task<UserModel?> LogIn(LoginRequest model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

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

        return user;
    }

    public async Task<UserModel?> LogInWithFacebook(string fbtoken)
    {
        var validationResult = await _facebookAuthService.ValidateAccessTokenAsync(fbtoken);
        if (validationResult is null || !validationResult.Data.IsValid) throw new AppException("token is invalid");

        var userInfo = await _facebookAuthService.GetUserInfoAsync(fbtoken);

        var cognitoUser = await ((CognitoUserManager<CognitoUser>)_userManager).FindByEmailAsync(userInfo.Email);

        var userName = (userInfo.FirstName == userInfo.LastName) ? userInfo.FirstName : userInfo.FirstName + " " + userInfo.LastName;

        // Create new one
        if (cognitoUser is null)
        {
            var newUser = _pool.GetUser(userInfo.Email);

            newUser.Attributes.Add(CognitoAttribute.Email.AttributeName, userInfo.Email);
            newUser.Attributes.Add(CognitoAttribute.Name.AttributeName, userName);
            var createdUser = await _userManager.CreateAsync(newUser);

            if (!createdUser.Succeeded)
                if (createdUser.Errors.Any()) throw new AppException(createdUser.Errors.FirstOrDefault().Description);
        }

        var user = new UserModel
        {
            Username = userName,
            Email = userInfo.Email
        };

        return user;
    }
}