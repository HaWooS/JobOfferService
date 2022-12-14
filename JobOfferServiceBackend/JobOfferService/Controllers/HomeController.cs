using JobOfferService.Models.Authentication;
using JobOfferService.Utilities.Authentication;
using JobOfferService.Utilities.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JobOfferService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IUserAuthenticator _userAuthenticator;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger _logger;

        public HomeController(IUserAuthenticator userAuthenticator, IJwtTokenGenerator jwtTokenGenerator, ILogger<HomeController> logger)
        {
            _userAuthenticator = userAuthenticator;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
        }

        /// <summary>
        /// Registers user in the system
        /// </summary>
        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] RegistrationUserModel registrationUserModel)
        {
            try
            {
                if (registrationUserModel != null && registrationUserModel.IsValid())
                {
                    var result = _userAuthenticator.RegisterUser(registrationUserModel);
                    if (result.Result)
                        return Ok();
                    else
                        return new BadRequestObjectResult(new { Result = result.Errors });
                }
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured when processing the Register request" + ex.StackTrace + "\n" + ex.Message);
                return BadRequest();
            }
        }

        /// <summary>
        /// Authenticates user in the system and generates the JWT token for the specified user
        /// </summary>
        [HttpPost]
        [Route("Authenticate")]
        [AllowAnonymous]
        public IActionResult Authenticate([FromBody] AuthUserModel authUserModel)
        {
            if (authUserModel != null && !string.IsNullOrEmpty(authUserModel.UserName) && !string.IsNullOrEmpty(authUserModel.Password))
            {
                try
                {
                    var authenticationResult = _userAuthenticator.AuthenticateUser(authUserModel);
                    if (authenticationResult != null && authenticationResult.Result)
                        return Ok(new { token = _jwtTokenGenerator.GenerateUserJwt(authUserModel), accountType = authenticationResult.AccountType, errors = authenticationResult.Errors });
                    else
                        return new BadRequestObjectResult(new { errors = authenticationResult.Errors });
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred when processing the Authenticate request" + ex.Message + "\n" + ex.StackTrace);
                    return BadRequest();
                }
            }
            else
                return BadRequest();
        }
    }
}
