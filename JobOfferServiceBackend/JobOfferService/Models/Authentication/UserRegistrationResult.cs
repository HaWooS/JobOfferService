namespace JobOfferService.Models.Authentication
{
    public class UserRegistrationResult
    {
        public bool Result { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
