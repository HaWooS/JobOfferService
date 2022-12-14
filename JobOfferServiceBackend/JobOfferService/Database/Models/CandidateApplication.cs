namespace JobOfferService.Database.Models
{
    public class CandidateApplication
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Pesel { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Expectations { get; set; }
        public bool Accepted { get; set; }
        public int JobAdvertisementId { get; set; }
        public virtual JobAdvertisement JobAdvertisement { get; set; }
    }
}
