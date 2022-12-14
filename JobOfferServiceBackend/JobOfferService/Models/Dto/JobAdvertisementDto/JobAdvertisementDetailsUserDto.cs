using JobOfferService.Database.Models;

namespace JobOfferService.Models.Dto.JobAdvertisementDetailsDto
{
    public class JobAdvertisementDetailsUserDto
    {
        public JobAdvertisementDetailsUserDto(JobAdvertisement advertisement)
        {
            this.Id = advertisement.Id;
            this.Name = advertisement.Name;
            this.CreatedAt = advertisement.CreatedAt;
            this.Description = advertisement.Description;
            this.Location = advertisement.Location;
            this.AdditionalInformation = advertisement.AdditionalInformation;
            this.EmployerName = advertisement.EmployerName;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string AdditionalInformation { get; set; }
        public string EmployerName { get; set; }
    }
}
