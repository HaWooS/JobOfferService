using JobOfferService.Database.Models;
using Newtonsoft.Json;

namespace JobOfferService.Models.Dto.JobAdvertisementDetailsDto
{
    public class JobAdvertisementDetailsAdminDto
    { 
        public JobAdvertisementDetailsAdminDto() { }
        public JobAdvertisementDetailsAdminDto(JobAdvertisement advertisement)
        {
            this.Id = advertisement.Id;
            this.Name = advertisement.Name;
            this.Description = advertisement.Description;
            this.Location = advertisement.Location;
            this.AdditionalInformation = advertisement.AdditionalInformation;
            this.MaximumNumberOfEmployees = advertisement.MaximumNumberOfEmployees;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string AdditionalInformation { get; set; }
        public int MaximumNumberOfEmployees { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.Name)) return false;
            if (string.IsNullOrEmpty(this.Description)) return false;
            if (string.IsNullOrEmpty(this.Location)) return false;
            if (string.IsNullOrEmpty(this.AdditionalInformation)) return false;
            if (this.MaximumNumberOfEmployees < 0) return false;

            return true;
        }
    }
}
