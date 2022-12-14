using JobOfferService.Database.Models;
using JobOfferService.Models.Authentication;

namespace JobOfferService.Utilities.Authentication
{
    public interface IUserAuthenticator 
    {
        public AppUser? GetUserByToken(string token);
        public UserAuthenticationResult? AuthenticateUser(AuthUserModel authUserModel);
        public UserRegistrationResult? RegisterUser(RegistrationUserModel registrationUserModel);
        public string GetUsernameFromToken(string token);
    }
}
