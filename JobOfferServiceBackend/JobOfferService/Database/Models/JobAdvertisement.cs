namespace JobOfferService.Database.Models
{
    public class JobAdvertisement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } 
        public string Description { get; set; }
        public string Location { get; set; }
        public string AdditionalInformation { get; set; }
        public string EmployerName { get; set; }
        public int EmployerId { get; set; }
        public int MaximumNumberOfEmployees { get; set; }
        public virtual AppUser Employer { get; set; }
        public virtual List<CandidateApplication> CandidateApplications { get; set; }
    }
}
