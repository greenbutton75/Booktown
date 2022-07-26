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
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IUserService userService,
            ILogger<AccountController> logger
            )
        {
            _userService = userService;
            _logger = logger;   
        }

        [HttpPost("Signup")]
        public async Task<IActionResult> Signup(SignupRequest model)
        {
            var userModel = await _userService.Signup(model);

            if (userModel == null)
            {
                _logger.LogError("Signup failed: {Email}", model.Email);
                throw new AppException("Signup failed");
            }

            return Ok(userModel);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            var AuthenticateResponse = await _userService.LogIn(model);

            if (AuthenticateResponse == null)
            {
                _logger.LogError("Login failed: {Email}", model.Email);
                throw new AppException("Login failed");
            }

            return Ok(AuthenticateResponse);
        }

        [HttpGet]
        [Authorize]
        [ResponseCache(Duration = 10)]
        public IActionResult Get()
        {
            return new JsonResult(new { name = "1", val = "3" , t = DateTime.Now.ToString() });
        }
    }
}