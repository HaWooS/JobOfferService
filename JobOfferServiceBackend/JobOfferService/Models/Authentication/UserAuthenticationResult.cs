namespace JobOfferService.Models.Authentication
{
    public class UserAuthenticationResult
    {
        public bool Result { get; set; }
        public int AccountType { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
