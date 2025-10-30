namespace GabriniCosmetics.Areas.Admin.Models.DTOs
{
    public class SliderBannerDTO
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
    }

    public class CreateSliderBannerDTO
    {
        public IFormFile ImageUpload { get; set; }
    }

    public class UpdateSliderBannerDTO
    {
        public int Id { get; set; }
        public IFormFile ImageUpload { get; set; }
        public string ExistingImagePath { get; set; }
    }
}
