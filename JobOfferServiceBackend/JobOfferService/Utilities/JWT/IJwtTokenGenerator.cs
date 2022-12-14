using JobOfferService.Models.Authentication;

namespace JobOfferService.Utilities.JWT
{
    public interface IJwtTokenGenerator
    {
        public string GenerateUserJwt(AuthUserModel authUserModel);
    }
}
