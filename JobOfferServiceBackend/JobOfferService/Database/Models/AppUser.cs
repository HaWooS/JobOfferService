namespace JobOfferService.Database.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public virtual List<JobAdvertisement> JobAdvertisements { get; set; }
        public AccountTypes AccountType { get; set; }
    }

    public enum AccountTypes
    {
        ADMIN,
        USER
    }
}
