using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GabriniCosmetics.Areas.Admin.Models.DTOs
{
    public class ProductDTO
    {
        public Product Product { get; set; }
        public List<Subproduct> Subproducts { get; set; }
        public List<ProductFlag> Flags { get; set; }
    }
    public class ProductForm
    {
        public int? Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public double Price { get; set; }
        public double? PriceAfterDiscount { get; set; }
        public int? PersantageSale { get; set; }
        public List<SubprodcutForm> Subproducts { get; set; } = new List<SubprodcutForm>();
        public List<int> Flags { get; set; }
        public List<ProductFlag> _Flags { get; set; }
        public List<string> FlagNames { get; set; }
        public string FlagNamesString { get; set; }
        public int SubcategoryId { get; set; }
        public bool IsDealOfDay { get; set; } = false;
        public bool IsSale { get; set; } = false;
    }

    public class SubprodcutForm
    {
        public int? Id { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public bool IsAvailability { get; set; }
        public IFormFile? Image { get; set; } = null;
        public string Base64Image { get; set; }
        public string? ExistingImagePath { get; set; }
        public int Order { get; set; }
    }

    public class CreateProductDTO
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public double Price { get; set; }
        public double? PriceAfterDiscount { get; set; }
        public int? PersantageSale { get; set; }
        public List<CreateSubproductDto> Subproducts { get; set; } = new List<CreateSubproductDto>();
        public List<int> Flags { get; set; } = new List<int>();
        public int SubcategoryId { get; set; }
        public bool IsDealOfDay { get; set; }
        //public bool IsAvailability { get; set; }
        public bool IsSale { get; set; }
        public List<string> FlagNames { get; set; } = new List<string>();
        public string? FlagNamesString { get; set; }
    }

    public class CreateSubproductDto
    {
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public bool IsAvailability { get; set; }
        public IFormFile? Image { get; set; } = null;
        public string Base64Image { get; set; }
    }

    public class ProductView
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public double Price { get; set; }
        public double? PriceAfterDiscount { get; set; }
        public int? PersantageSale { get; set; }
        public List<SubproductView> Subproducts { get; set; }
        //public List<int> Flags { get; set; }
        public List<FlagView> Flags { get; set; } = new List<FlagView>();
        public List<string> FlagNames { get; set; }
        public string FlagNamesString { get; set; }
        //public List<string> ExistingImagePaths { get; set; }
        public int SubcategoryId { get; set; }
        public bool IsDealOfDay { get; set; }
        public bool IsSale { get; set; }
        public SubcategoryView Subcategory { get; set; }
        //public bool IsAvailability { get; set; }
    }

    public class SubcategoryView
    {
        public int Id { get; set; }

        public string NameEn { get; set; }

        public string NameAr { get; set; }

        public int CategoryId { get; set; }

        public CategoryView Category { get; set; }
    }

    public class CategoryView
    {
        public int Id { get; set; }

        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string ImageUpload { get; set; }
    }

    public class SubproductView
    {
        public int? Id { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public bool IsAvailability { get; set; }
        public ImageView Image { get; set; }
        public int Order { get; set; }
    }

    public class ImageView
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
    }

    public class FlagView
    {
        public int Id { get; set; }
        public string FlagType { get; set; }
    }
}
