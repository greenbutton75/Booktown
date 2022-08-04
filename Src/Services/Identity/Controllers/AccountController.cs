using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Identity.Helpers;
using Identity.Models;
using Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    [ApiController]
    [Route("accounts/v1")]
    [Produces("application/json")]
    public class AccountController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IUserService userService,
            ITokenService tokenService,
            ILogger<AccountController> logger
            )
        {
            _userService = userService;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost("Signup")]
        public async Task<ActionResult<AuthenticateResponse>> Signup(SignupRequest model)
        {
            var userModel = await _userService.Signup(model);

            if (userModel == null)
            {
                _logger.LogError("Signup failed: {Email}", model.Email);
                throw new AppException("Signup failed");
            }

            var tokenModel = await _tokenService.GenerateJwtTokens(userModel);

            return Ok(new AuthenticateResponse(
                new UserModel
                {
                    Email = userModel.Email,
                    Username = userModel.Username
                },
                tokenModel.Token!,
                tokenModel.RefreshToken!
            ));
        }
        [HttpPost("Login")]
        public async Task<ActionResult<AuthenticateResponse>> Login(LoginRequest model)
        {
            var AuthenticateResponse = await _userService.LogIn(model);

            if (AuthenticateResponse == null)
            {
                _logger.LogError("Login failed: {Email}", model.Email);
                throw new AppException("Login failed");
            }

            return Ok(AuthenticateResponse);
        }
        [HttpPost("RevokeRefreshTokens")]
        public async Task<ActionResult> RevokeRefreshTokens(string token)
        {
            await _tokenService.RevokeRefreshTokens(token);

            return Ok();
        }
        [HttpPost("GetRefreshToken")]
        public async Task<ActionResult<TokenModel>> GetRefreshToken(TokenModel tokenModel)
        {
            var tokenModelResponse = await _tokenService.GetRefreshToken(tokenModel);

            return Ok(tokenModelResponse);
        }

        [HttpPost("LogInWithFacebook")]
        public async Task<ActionResult<AuthenticateResponse>> LogInWithFacebook(string token)
        {
            var AuthenticateResponse = await _userService.LogInWithFacebook(token);

            if (AuthenticateResponse == null)
            {
                _logger.LogError("Login Facebook failed");
                throw new AppException("Login failed");
            }

            return Ok(AuthenticateResponse);
        }

        [HttpGet]
        [Authorize]
        [ResponseCache(Duration = 10)]
        public IActionResult Get()
        {
            return new JsonResult(new { name = "1", val = "3", t = DateTime.Now.ToString() });
        }
    }
}