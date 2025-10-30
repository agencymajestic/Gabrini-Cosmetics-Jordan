using GabriniCosmetics.Areas.Admin.Models.DTOs;

namespace GabriniCosmetics.Areas.Admin.Models.Interface
{
    public interface IProduct
    {
        Task<List<ProductView>> GetProducts();
        Task<List<ProductView>> GetProductsBySubcategory(int subcategoryId);
        Task<ProductView> CreateProduct(ProductForm createProductDto);
        //Task<ProductDTO> GetProductById(int id);
        //Task<List<ProductDTO>> GetProductBySubCategory(int id);
        Task<ProductView> UpdateProduct(ProductForm updateProductDto);
        Task<bool> DeleteProduct(int id);
        IQueryable<Product> Queryable();
        Task<int> GetProductsCountAsync();
        Task<int> GetTotalImagesCountAsync();
        Task<int> GetNewProductsCountAsync();
        Task<int> GetSaleProductsCountAsync();
        Task<int> GetFeatureProductsCountAsync();
        Task<List<Product>> GetProductsByIdsAsync(List<int> productIds);
        Task<ProductView> GetProductById(int id);

    }
}
