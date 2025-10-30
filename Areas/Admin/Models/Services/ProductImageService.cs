using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Data;
using Microsoft.EntityFrameworkCore;

namespace GabriniCosmetics.Areas.Admin.Models.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly GabriniCosmeticsContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ProductImageService> _logger;

        public ProductImageService(GabriniCosmeticsContext context, IWebHostEnvironment environment, ILogger<ProductImageService> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        public async Task<ProductImage> AddImagesAsync(Subproduct subproduct, IFormFile imageUploads)
        {
            try
            {
                    var imagePath = await SaveImageAsync(imageUploads);
                    var productImage = new ProductImage
                    {
                        ImagePath = imagePath,
                        SubproductId = subproduct.Id,
                        Subproduct = subproduct
                    };

                //subproduct.Image = productImage;

                return productImage;

                //await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding images");
                throw;
            }
        }

        public async Task RemoveImagesAsync(int subproductId)
        {
            try
            {
                // Find the product image in the database
                var productImage = await _context.ProductImages.SingleOrDefaultAsync(pi => pi.SubproductId == subproductId);

                    if (productImage != null)
                    {
                        // Remove the image file from the file system
                        var filePath = Path.Combine(_environment.WebRootPath, "uploads", productImage.ImagePath);
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }

                        // Remove the image record from the database
                        _context.ProductImages.Remove(productImage);
                    }
                    else
                    {
                        _logger.LogWarning("Image not found: {ImagePath}", productImage.ImagePath);
                    }
                
                // Save changes after processing all images
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing images");
                throw;
            }
        }

        private async Task<string> SaveImageAsync(IFormFile imageUpload)
        {
            try
            {
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageUpload.FileName)}";
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageUpload.CopyToAsync(fileStream);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving image");
                throw;
            }
        }
    }

}
