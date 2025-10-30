namespace GabriniCosmetics.Areas.Admin.Models
{
    public class Flag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation property for products
        public ICollection<ProductFlag> ProductFlags { get; set; } // Junction entity

        // Constructor to initialize collections
        public Flag()
        {
            ProductFlags = new List<ProductFlag>();
        }
    }
    public class FlagConvert
    {
        public string value { get; set; }
    }
}
