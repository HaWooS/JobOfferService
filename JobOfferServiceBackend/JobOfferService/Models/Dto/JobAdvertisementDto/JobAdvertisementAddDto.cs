namespace JobOfferService.Models.Dto.JobAdvertisementDto
{
    public class JobAdvertisementAddDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string AdditionalInformation { get; set; }
        public int MaximumNumberOfEmployees { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Name)) return false;
            if (string.IsNullOrEmpty(Description)) return false;
            if (string.IsNullOrEmpty(Location)) return false;
            if (string.IsNullOrEmpty(AdditionalInformation)) return false;
            if (this.MaximumNumberOfEmployees < 1) return false;

            return true;
        }
    }
}
