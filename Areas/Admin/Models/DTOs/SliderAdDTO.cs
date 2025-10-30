namespace GabriniCosmetics.Areas.Admin.Models.DTOs
{
    public class SliderAdDTO
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Link { get; set; }
    }

    public class CreateSliderAdDTO
    {
        public IFormFile ImageUpload { get; set; }
        public string Link { get; set; }
    }

    public class UpdateSliderAdDTO
    {
        public int Id { get; set; }
        public IFormFile ImageUpload { get; set; }
        public string ExistingImagePath { get; set; }
        public string Link { get; set; }
    }

}
