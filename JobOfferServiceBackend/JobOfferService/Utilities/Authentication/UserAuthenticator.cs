using JobOfferService.Database.Context;
using JobOfferService.Database.Models;
using JobOfferService.Models.Authentication;
using JobOfferService.Utilities.JWT;
using JobOfferService.Utilities.PasswordClient;

namespace JobOfferService.Utilities.Authentication
{
    public class UserAuthenticator : IUserAuthenticator
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserHandler _userHandler;
        private readonly ILogger _logger;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public UserAuthenticator(ApplicationDbContext context, IUserHandler userHandler, ILogger<UserAuthenticator> logger, IJwtTokenGenerator tokenGenerator)
        {
            _context = context;
            _userHandler = userHandler;
            _logger = logger;
            _tokenGenerator = tokenGenerator;
        }
        
        public string GetUsernameFromToken(string token)
        {
            var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().ReadJwtToken(token);
            string userName = jwt.Claims.First(c => c.Type == "sub").Value ?? "";
            return userName;
        }
        public AppUser? GetUserByToken(string token)
        {
            var userName = this.GetUsernameFromToken(token);

            return _context.Users.FirstOrDefault(x => x.Name == userName);
        }

        public UserRegistrationResult? RegisterUser(RegistrationUserModel registrationUserModel)
        {
            try
            {
                if (registrationUserModel != null && registrationUserModel.IsValid())
                {
                    UserRegistrationResult result = new UserRegistrationResult();

                    var existingUser = _context.Users.Where(x => x.Name == registrationUserModel.UserName).FirstOrDefault();
                    if (existingUser != null)
                    {
                        result.Errors.Add($"User with the login of {registrationUserModel.UserName} exists in the database");
                        result.Result = false;
                        return result;
                    }

                    var newUser = new AppUser()
                    {
                        Email = registrationUserModel.Email,
                        Name = registrationUserModel.UserName,
                        Password = _userHandler.HashUserPassword(registrationUserModel.Password),
                        AccountType = registrationUserModel.AccountType
                    };

                    _context.Users.Add(newUser);
                    _context.SaveChanges();

                    result.Result = true;
                    return result;

                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred during the registration process" + ex.StackTrace + "\n" + ex.Message);
                return null;
            }
        }

        public UserAuthenticationResult? AuthenticateUser(AuthUserModel authUserModel)
        {
            try
            {
                if (authUserModel != null && !string.IsNullOrEmpty(authUserModel.UserName) && !string.IsNullOrEmpty(authUserModel.Password))
                {
                    UserAuthenticationResult userAuthenticationResult = new UserAuthenticationResult();

                    var dbUser = _context.Users.FirstOrDefault(x => x.Name == authUserModel.UserName);
                    if (dbUser != null)
                    {
                        userAuthenticationResult.Result = _userHandler.VerifyUserPassword(dbUser.Password, authUserModel.Password) ? true : false;
                        if (!userAuthenticationResult.Result)
                            userAuthenticationResult.Errors.Add("Invalid username or password");

                        if (userAuthenticationResult.Result) userAuthenticationResult.AccountType = dbUser.AccountType == AccountTypes.ADMIN ? 0 : 1;

                        return userAuthenticationResult;
                    }
                    else
                    {
                        userAuthenticationResult.Errors.Add($"User with the login of {authUserModel.UserName} does not exists in the database");
                        userAuthenticationResult.Result = false;

                        return userAuthenticationResult;
                    }
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred during the authentication process" + ex.StackTrace + "\n" + ex.Message);
                return null;
            }
        }
    }
}
