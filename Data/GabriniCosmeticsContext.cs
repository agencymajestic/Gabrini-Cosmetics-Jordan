using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GabriniCosmetics.Areas.Admin.Models.CustomerInfo;
using GabriniCosmetics.Areas.Admin.Models;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using System.Linq.Expressions;

namespace GabriniCosmetics.Data
{
    public class GabriniCosmeticsContext : IdentityDbContext<ApplicationUser>
    {
        public GabriniCosmeticsContext(DbContextOptions<GabriniCosmeticsContext> options)
                : base(options)
        {
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subcategory> Subcategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductFlag> ProductFlags { get; set; }
        public DbSet<Flag> Flags { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<WishlistDetail> WishlistDetail { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<SliderBanner> SliderBanners { get; set; }
        public DbSet<SliderAd> SliderAds { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<AnnouncementBar> AnnouncementBars { get; set; }
        public DbSet<DealOfTheDays> DealOfTheDays { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Subproduct> Subproducts { get; set; }
        public DbSet<SocialMedia> SocialMedias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Subproduct>().HasQueryFilter(sp => !sp.IsDeleted);
        }

    }
}
