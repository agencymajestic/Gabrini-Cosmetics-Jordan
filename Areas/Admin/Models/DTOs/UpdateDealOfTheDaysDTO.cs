namespace GabriniCosmetics.Areas.Admin.Models.DTOs
{
    public class UpdateDealOfTheDaysDTO
    {
        public int Id { get; set; }
        public DateTime EndTime { get; set; }
        public IFormFile ImageUpload { get; set; }
        public string ExistingImagePath { get; set; }
    }
}