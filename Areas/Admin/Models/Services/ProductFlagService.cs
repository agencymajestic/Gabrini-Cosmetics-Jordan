using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Data;

namespace GabriniCosmetics.Areas.Admin.Models.Services
{
    public class ProductFlagService : IProductFlagService
    {
        private readonly GabriniCosmeticsContext _context;
        private readonly ILogger<ProductFlagService> _logger;

        public ProductFlagService(GabriniCosmeticsContext context, ILogger<ProductFlagService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddFlagsAsync(Product product, List<int> flags)
        {
            try
            {
                // Remove existing flags associated with the product
                var existingFlags = _context.ProductFlags.Where(pf => pf.ProductId == product.Id).ToList();
                _context.ProductFlags.RemoveRange(existingFlags);

                // Add new flags from the update DTO
                foreach (var flagId in flags)
                {
                    var flag = await _context.Flags.FindAsync(flagId);
                    if (flag != null)
                    {
                        var productFlag = new ProductFlag
                        {
                            FlagType = flag.Name,
                            ProductId = product.Id,
                            Product = product
                        };
                        product.Flags.Add(productFlag);
                    }
                    else
                    {
                        _logger.LogWarning($"Flag with id {flagId} not found.");
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding flags");
                throw;
            }
        }

        public async Task DeleteFlagsAsync(int productID)
        {
            var existingFlags = _context.ProductFlags.Where(pf => pf.ProductId == productID).ToList();
            _context.ProductFlags.RemoveRange(existingFlags);
            await _context.SaveChangesAsync();
        }
    }

}
