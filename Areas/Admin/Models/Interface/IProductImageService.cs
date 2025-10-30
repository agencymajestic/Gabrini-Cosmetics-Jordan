namespace GabriniCosmetics.Areas.Admin.Models.Interface
{
    public interface IProductImageService
    {
        Task<ProductImage> AddImagesAsync(Subproduct subproduct, IFormFile imageUploads);
        Task RemoveImagesAsync(int subproductId);
    }
}
