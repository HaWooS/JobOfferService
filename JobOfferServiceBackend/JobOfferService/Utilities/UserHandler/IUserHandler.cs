using JobOfferService.Database.Models;
using JobOfferService.Models.Authentication;
using JobOfferService.Utilities.UserHandler;

namespace JobOfferService.Utilities.PasswordClient
{
    public interface IUserHandler
    {
        public string HashUserPassword(string password);
        public bool VerifyUserPassword(string userInputPassword, string dbPassword);
    }
}
