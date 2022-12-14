using JobOfferService.Database.Models;

namespace JobOfferService.Models.Dto.JobAdvertisementDto
{
    public class JobAdvertisementListDto
    {
        public JobAdvertisementListDto(JobAdvertisement advertisement)
        {
            this.Id = advertisement.Id;
            this.Name = advertisement.Name;
            this.CreatedAt = advertisement.CreatedAt;
            this.Location = advertisement.Location;
            this.EmployerName = advertisement.EmployerName;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Location { get; set; }
        public string EmployerName { get; set; }
    }
}
