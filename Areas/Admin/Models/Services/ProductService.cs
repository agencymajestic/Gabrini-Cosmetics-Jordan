using AutoMapper;
using AutoMapper.QueryableExtensions;
using GabriniCosmetics.Areas.Admin.Models.DTOs;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Data;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace GabriniCosmetics.Areas.Admin.Models.Services
{
    public class ProductService : IProduct
    {
        private readonly GabriniCosmeticsContext _context;
        private readonly ILogger<ProductService> _logger;
        private readonly IProductImageService _productImageService;
        private readonly IProductFlagService _productFlagService;
        private readonly IMapper _mapper;

        public ProductService(GabriniCosmeticsContext context, ILogger<ProductService> logger,
                              IProductImageService productImageService, IProductFlagService productFlagService, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _productImageService = productImageService;
            _productFlagService = productFlagService;
            _mapper = mapper;
        }

        public async Task<List<ProductView>> GetProducts()
        {
            try
            {
                return await _context.Products.ProjectTo<ProductView>(_mapper.ConfigurationProvider).OrderBy(p => p.NameEn).ThenBy(p => p.NameAr).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                throw;
            }
        }

        public async Task<List<ProductView>> GetProductsBySubcategory(int subcategoryId)
        {
            try
            {
                return await _context.Products.ProjectTo<ProductView>(_mapper.ConfigurationProvider).Where(p => p.SubcategoryId == subcategoryId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting products for subcategory id {subcategoryId}");
                throw;
                throw;
            }
        }

        public async Task<ProductView> GetProductById(int id)
        {
            try
            {
                return await _context.Products.ProjectTo<ProductView>(_mapper.ConfigurationProvider).SingleOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting product by id {id}");
                throw;
            }
        }


        //public async Task<ProductDTO> GetProductById(int id)
        //{
        //    try
        //    {
        //        var product = await _context.Products
        //            .Include(p => p.Subproducts)
        //            .ThenInclude(sp => sp.Image)
        //            .Include(p => p.Flags)
        //            .Select(p => new ProductDTO
        //            {
        //                Product = p,
        //                Subproducts = p.Subproducts.ToList(),
        //                Flags = p.Flags.ToList()
        //            })
        //            .FirstOrDefaultAsync(p => p.Product.Id == id);

        //        return product ?? throw new KeyNotFoundException($"Product with id {id} not found.");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error getting product by id {id}");
        //        throw;
        //    }
        //}
        //public async Task<List<ProductDTO>> GetProductBySubCategory(int id)
        //{
        //    try
        //    {
        //        var products = await _context.Products
        //            .Include (p => p.Subproducts)
        //            .ThenInclude(sp => sp.Image)
        //            .Include(p => p.Flags)
        //            .Where(p => p.SubcategoryId == id)
        //            .Select(p => new ProductDTO
        //            {
        //                Product = p,
        //                Subproducts = p.Subproducts.ToList(),
        //                Flags = p.Flags.ToList()
        //            })
        //            .ToListAsync();

        //        return products;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error getting products for subcategory id {id}");
        //        throw;
        //    }
        //}

        public async Task<ProductView> CreateProduct(ProductForm createProductDto)
        {
            try
            {
                if(!createProductDto.Subproducts.Any())
                {
                    throw new Exception("Subproducts cannot be empty");
                }

                var subproducts = new List<Subproduct>();

                foreach(var subproductDto in createProductDto.Subproducts)
                {
                    var subproduct = new Subproduct
                    {
                        DescriptionAr = subproductDto.DescriptionAr,
                        DescriptionEn = subproductDto.DescriptionEn,
                        IsAvailability = subproductDto.IsAvailability,
                        Order = subproductDto.Order
                    };

                    var image = await _productImageService.AddImagesAsync(subproduct, subproductDto.Image);

                    subproduct.Image = image;

                    subproducts.Add(subproduct);
                }

                var product = new Product
                {
                    NameEn = createProductDto.NameEn,
                    NameAr = createProductDto.NameAr,
                    DescriptionEn = createProductDto.DescriptionEn,
                    DescriptionAr = createProductDto.DescriptionAr,
                    Price = (double)createProductDto.Price,
                    PriceAfterDiscount = (double?)createProductDto.PriceAfterDiscount,
                    PersantageSale = createProductDto.PersantageSale,
                    SubcategoryId = createProductDto.SubcategoryId,
                    Subproducts = subproducts,
                    Flags = new List<ProductFlag>(),
                    IsDealOfDay = createProductDto.IsDealOfDay,
                    IsSale = createProductDto.IsSale,
                    FlagNamesString = createProductDto.FlagNamesString
                };



                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                //if (createProductDto.ImageUploads != null && createProductDto.ImageUploads.Any())
                //{
                //    await _productImageService.AddImagesAsync(product, createProductDto.ImageUploads);
                //}

                if (createProductDto.Flags != null && createProductDto.Flags.Any())
                {
                    await _productFlagService.AddFlagsAsync(product, createProductDto.Flags);
                }

                return _mapper.Map<ProductView>(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                throw;
            }
        }

        public async Task<ProductView> UpdateProduct(ProductForm updateProductDto)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                if (!updateProductDto.Subproducts.Any())
                {
                    throw new Exception("Subproducts cannot be empty");
                }

                var existingProduct = await _context.Products
                    .Include(p => p.Subproducts)
                    .ThenInclude(sp => sp.Image)
                    .Include(p => p.Flags)
                    .FirstOrDefaultAsync(p => p.Id == updateProductDto.Id);

                if (existingProduct == null)
                {
                    throw new KeyNotFoundException($"Product with id {updateProductDto.Id} not found.");
                }

                var subproducts = new List<Subproduct>();

                foreach (var subproductDto in updateProductDto.Subproducts)
                {
                    // if subproductDto Id is NULL then it's a newly added subproduct and is set to 0
                    var subproduct = new Subproduct
                    {   
                        Id = subproductDto.Id ?? 0,
                        DescriptionAr = subproductDto.DescriptionAr,
                        DescriptionEn = subproductDto.DescriptionEn,
                        IsAvailability = subproductDto.IsAvailability,
                        ProductId = existingProduct.Id,
                        Order = subproductDto.Order
                    };

                    if(subproductDto.Image is not null)
                    {
                        try
                        {
                            //new subproducts don't have images to be removed
                            if(subproduct.Id != 0) 
                            {
                                await _productImageService.RemoveImagesAsync(subproduct.Id);
                            }

                            var image = await _productImageService.AddImagesAsync(subproduct, subproductDto.Image);

                            subproduct.Image = image;
                        }
                        catch (Exception)
                        {
                            await transaction.RollbackAsync();
                            throw new Exception("error ocurred while saving new images");
                        }
                    }

                    if(subproduct.Id == 0)
                    {
                        await _context.Subproducts.AddAsync(subproduct);
                    }
                    else
                    {
                        Subproduct existingSubproduct = existingProduct.Subproducts.SingleOrDefault(sp => sp.Id == subproduct.Id)!;
                        if(existingSubproduct is not null)
                        {
                            _context.Subproducts.Entry(existingSubproduct).State = EntityState.Detached;
                            _context.Subproducts.Update(subproduct);
                        }
                    }

                    subproducts.Add(subproduct);
                }

                existingProduct.NameEn = updateProductDto.NameEn;
                existingProduct.NameAr = updateProductDto.NameAr;
                existingProduct.DescriptionEn = updateProductDto.DescriptionEn;
                existingProduct.DescriptionAr = updateProductDto.DescriptionAr;
                existingProduct.Price = (double)updateProductDto.Price;
                existingProduct.PriceAfterDiscount = (double?)updateProductDto.PriceAfterDiscount;
                existingProduct.PersantageSale = updateProductDto.PersantageSale;
                existingProduct.SubcategoryId = updateProductDto.SubcategoryId;
                existingProduct.IsDealOfDay = updateProductDto.IsDealOfDay;
                existingProduct.IsSale = updateProductDto.IsSale;
                existingProduct.Subproducts = subproducts;

                //// Handle existing images - Remove images that are not in ExistingImagePaths
                //if (updateProductDto.ExistingImagePaths != null)
                //{
                //    var imagesToRemove = existingProduct.Images
                //        .Where(image => !updateProductDto.ExistingImagePaths.Contains(image.ImagePath))
                //        .Select(image => image.ImagePath)
                //        .ToList();

                //    if (imagesToRemove.Any())
                //    {
                //        await _productImageService.RemoveImagesAsync(existingProduct.Id, imagesToRemove);
                //    }
                //}
                //else
                //{
                //    // If no existing image paths are provided, remove all images
                //    var allImages = existingProduct.Images.Select(x => x.ImagePath).ToList();
                //    await _productImageService.RemoveImagesAsync(existingProduct.Id, allImages);
                //}

                //// Add new images if provided
                //if (updateProductDto.ImageUploads != null && updateProductDto.ImageUploads.Any())
                //{
                //    await _productImageService.AddImagesAsync(existingProduct, updateProductDto.ImageUploads);
                //}


                // Handle flags
                if (updateProductDto.Flags != null && updateProductDto.Flags.Any())
                {
                    await _productFlagService.AddFlagsAsync(existingProduct, updateProductDto.Flags);
                }
                else
                {
                    await _productFlagService.DeleteFlagsAsync(existingProduct.Id);
                }

                    await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<ProductView>(existingProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product with id {updateProductDto.Id}");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Subproducts)
                    .ThenInclude(sp => sp.Image)
                    .Include(p => p.Flags)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                    return false;

                //var images = _context.Subproducts.Select(p => p.Image).ToList();
                //_context.ProductImages.RemoveRange(images);
                //_context.Subproducts.RemoveRange(product.Subproducts);
                //_context.ProductFlags.RemoveRange(product.Flags);
                //_context.Products.Remove(product);

                product.IsDeleted = true;
                foreach (var subproduct in product.Subproducts)
                {
                    subproduct.IsDeleted = true;
                }

                _context.Update(product);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product with id {id}");
                throw;
            }
        }

        public IQueryable<Product> Queryable()
        {
            return _context.Set<Product>().AsQueryable();
        }

        public async Task<int> GetProductsCountAsync()
        {
            return await _context.Products.CountAsync();
        }
        public async Task<int> GetNewProductsCountAsync()
        {
            return await _context.Products
                .Where(p => p.Flags.Any(x => x.FlagType == "New"))
                .CountAsync();
        }
        public async Task<int> GetSaleProductsCountAsync()
        {
            return await _context.Products
                .Where(p => p.Flags.Any(x => x.FlagType == "Sale"))
                .CountAsync();
        }
        public async Task<int> GetFeatureProductsCountAsync()
        {
            return await _context.Products
                .Where(p => p.Flags.Any(x => x.FlagType == "Feature"))
                .CountAsync();
        }

        public async Task<List<Product>> GetProductsByIdsAsync(List<int> productIds)
        {
            return await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();
        }

        public async Task<int> GetTotalImagesCountAsync()
        {
            try
            {
                return await _context.Subproducts.CountAsync();

                // Fetching all image counts first and then summing them up
                //var imageCounts = await _context.Products
                //    .Include(p => p.Images)
                //    .Select(p => p.Images.Count) // Selecting image counts for each product
                //    .ToListAsync();

                //return imageCounts.Sum(); // Summing up all the counts
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total image count across products");
                throw;
            }
        }
    }
}
