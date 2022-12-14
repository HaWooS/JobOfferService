using JobOfferService.Database.Models;

namespace JobOfferService.Models.Authentication
{
    public class RegistrationUserModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public AccountTypes AccountType { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(UserName)) return false;
            if (string.IsNullOrEmpty(Password)) return false;
            if (string.IsNullOrEmpty(Email)) return false;
            return true;
        }
    }
}
