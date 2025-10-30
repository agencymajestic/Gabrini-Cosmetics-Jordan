using GabriniCosmetics.Areas.Admin.Models.DTOs;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GabriniCosmetics.Areas.Admin.Models.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly GabriniCosmeticsContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public WishlistService(GabriniCosmeticsContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<int> AddItem(int subproductId)
        {
            string userId = GetUserId();
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null.");
            }

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var wishlist = await GetOrCreateWishlist(userId);

                //var productDTO = await _product.GetProductById(productId);
                var subproduct = await _db.Subproducts.Include(sp => sp.Image).SingleOrDefaultAsync(sp => sp.Id == subproductId);
                //var product = new Product()
                //{
                //    Id = productDTO.Product.Id,
                //    NameEn = productDTO.Product.NameEn,
                //    NameAr = productDTO.Product.NameAr,
                //    DescriptionEn = productDTO.Product.DescriptionEn,
                //    DescriptionAr = productDTO.Product.DescriptionAr,

                //    Flags = productDTO.Product.Flags,
                //    Images = productDTO.Product.Images,

                //    Subcategory = productDTO.Product.Subcategory,
                //    SubcategoryId = productDTO.Product.SubcategoryId,

                //    Price = productDTO.Product.Price,
                //    PriceAfterDiscount = productDTO.Product.PriceAfterDiscount,
                //    PersantageSale = productDTO.Product.PersantageSale
                //};

                await UpdateWishlistItem(subproduct.Id, wishlist, 1);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately (e.g., log or throw)
            }

            var wishlistItemCount = await GetWishlistItemCount();
            return wishlistItemCount;
        }
        public async Task<int> AddItem(int subproductId, int qty)
        {
            string userId = GetUserId();
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User is not logged in");

                var wishlist = await GetOrCreateWishlist(userId);
                await UpdateWishlistItem(subproductId, wishlist, qty);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately (e.g., log or throw)
            }

            var wishlistItemCount = await GetWishlistItemCount();
            return wishlistItemCount;
        }

        public async Task<int> RemoveItem(int subproductId)
        {
            string userId = GetUserId();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User is not logged in");

                var wishlist = await GetWishlist(userId);
                if (wishlist == null)
                    throw new Exception("Wishlist not found");

                var wishlistItem = wishlist.WishlistsDetail.Where(a => a.SubproductId == subproductId).FirstOrDefault();
                if (wishlistItem != null)
                {
                    if (wishlistItem.Quantity > 1)
                        wishlistItem.Quantity -= 1;
                    else
                        _db.WishlistDetail.Remove(wishlistItem);
                }

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately (e.g., log or throw)
            }

            var wishlistItemCount = await GetWishlistItemCount();
            return wishlistItemCount;
        }

        public async Task<Wishlist> GetUserWishlist()
        {
            string userId = GetUserId();
            if (userId == null)
            {
                // Handle the case where userId is null (e.g., redirect to login or show an error message)
                // For now, let's return an empty wishlist or some default response
                var emptyWishlist = new Wishlist(); // You need to replace this with your actual Wishlist class
                return emptyWishlist;
            }
            else
            {
                var wishlist = await GetOrCreateWishlist(userId);
                return wishlist;
            }
        }

        public async Task<Wishlist> GetUnknownWishList(Guid wishListGroup)
        {
                var wishList = await GetOrCreateWishlistForUnknownUser(wishListGroup);
                return wishList;
        }

        public async Task<IEnumerable<WishlistDetail>> GetWishlistProductByUserId(string userId)
        {
            var wishlist = await GetWishlist(userId);
            if (wishlist == null)
                throw new Exception("Invalid wishlist");

            return wishlist.WishlistsDetail.ToList();
        }

        public async Task RemoveWishlistProducts(IEnumerable<WishlistDetail> wishlistProducts)
        {
            _db.WishlistDetail.RemoveRange(wishlistProducts);
            await _db.SaveChangesAsync();
        }

        public async Task<Wishlist> GetWishlist(string userId)
        {
            return await _db.Wishlists
                .Include(w => w.WishlistsDetail)
                    .ThenInclude(d => d.Subproduct)
                        .ThenInclude(sp => sp.Product)
                            .ThenInclude(p => p.Subcategory)
                                .ThenInclude(p => p.Category)
                .Include(w => w.WishlistsDetail)
                    .ThenInclude(d => d.Subproduct)
                        .ThenInclude(sp => sp.Product)
                            .ThenInclude(p => p.Flags)
                 .Include(w => w.WishlistsDetail)
                    .ThenInclude(d => d.Subproduct)
                        .ThenInclude(sp => sp.Image)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<Wishlist> GetUnknownUserWishlist(Guid wishListGroup)
        {
            return await _db.Wishlists
                .Include(w => w.WishlistsDetail)
                    .ThenInclude(d => d.Subproduct)
                        .ThenInclude(sp => sp.Product)
                            .ThenInclude(p => p.Subcategory)
                                .ThenInclude(p => p.Category)
                .Include(w => w.WishlistsDetail)
                    .ThenInclude(d => d.Subproduct)
                        .ThenInclude(sp => sp.Product)
                            .ThenInclude(p => p.Flags)
                 .Include(w => w.WishlistsDetail)
                    .ThenInclude(d => d.Subproduct)
                        .ThenInclude(sp => sp.Image)
                .FirstOrDefaultAsync(w => w.WishListGroup == wishListGroup);
        }

        public async Task<int> GetWishlistItemCount()
        {
            string userId = GetUserId();
            var wishlist = await GetWishlist(userId);
            return wishlist?.WishlistsDetail.Sum(d => d.Quantity) ?? 0;
        }

        private async Task<Wishlist> GetOrCreateWishlist(string userId)
        {
            // Fetch existing wishlist
            var wishlist = await GetWishlist(userId);

            // If no existing wishlist is found, create a new one
            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    UserId = userId,
                    // Initialize WishlistsDetail as an empty list
                    WishlistsDetail = new List<WishlistDetail>()
                };
                // Add the new wishlist to the database
                _db.Wishlists.Add(wishlist);
                await _db.SaveChangesAsync();
            }

            return wishlist;
        }

        private async Task<Wishlist> GetOrCreateWishlistForUnknownUser(Guid wishListGroup)
        {
            var wishList = await GetUnknownUserWishlist(wishListGroup);
            if (wishList is null)
            {
                var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == "unknown@GabriniCosmetics.com") ?? throw new Exception("Unable To Create Cart");
                wishList = new Wishlist
                {
                    UserId = user.Id,
                    WishlistsDetail = new List<WishlistDetail>(),
                    WishListGroup = wishListGroup
                };

                _db.Wishlists.Add(wishList);
                await _db.SaveChangesAsync();
            }

            return wishList;
        }

        private async Task UpdateWishlistItem(int subproductId, Wishlist wishlist, int qty)
        {
            //if (string.IsNullOrEmpty(img))
            //{
            //    throw new ArgumentNullException(nameof(img), "Image URL cannot be null or empty.");
            //}

            var wishlistItem = wishlist.WishlistsDetail.SingleOrDefault(w => w.SubproductId == subproductId);
            if (wishlistItem != null)
            {
                //if (wishlistItem.Image != img)
                //{
                //    var subproduct = await _db.Subproducts.Include(sp => sp.Product).Include(sp => sp.Image).SingleOrDefaultAsync(sp => sp.Id == subproductId);
                //    //var productDTO = await _product.GetProductById(productId);
                //    //var product = new Product()
                //    //{
                //    //    Id = productDTO.Product.Id,
                //    //    NameEn = productDTO.Product.NameEn,
                //    //    NameAr = productDTO.Product.NameAr,
                //    //    DescriptionEn = productDTO.Product.DescriptionEn,
                //    //    DescriptionAr = productDTO.Product.DescriptionAr,

                //    //    Flags = productDTO.Product.Flags,
                //    //    Images = productDTO.Product.Images,

                //    //    Subcategory = productDTO.Product.Subcategory,
                //    //    SubcategoryId = productDTO.Product.SubcategoryId,
                //    //    Price = productDTO.Product.Price,
                //    //    PriceAfterDiscount = productDTO.Product.PriceAfterDiscount,
                //    //    PersantageSale = productDTO.Product.PersantageSale
                //    //};

                //    // Attach the existing product instance if it's already being tracked
                //    var trackedProduct = _db.Subproducts.Local.FirstOrDefault(p => p.Id == subproduct.Id);
                //    if (trackedProduct != null)
                //    {
                //        _db.Entry(trackedProduct).State = EntityState.Detached;
                //    }

                //    _db.Attach(subproduct);

                //    wishlistItem = new WishlistDetail
                //    {
                //        SubproductId = subproduct.Id,
                //        WishlistId = wishlist.Id,
                //        Quantity = qty,
                //        Image = img ?? wishlistItem.Image,
                //        Subproduct = subproduct,
                //        Wishlist = wishlist,
                //        UnitPrice = subproduct.Product?.Price ?? 0,
                //    };


                //    _db.WishlistDetail.Add(wishlistItem);
                //}
                //else
                //{
                    wishlistItem.Quantity += qty;
                //}
            }
            else
            {
                try
                {
                    //var productDTO = await _product.GetProductById(productId);
                    //var product = new Product()
                    //{
                    //    Id = productDTO.Product.Id,
                    //    NameEn = productDTO.Product.NameEn,
                    //    NameAr = productDTO.Product.NameAr,
                    //    DescriptionEn = productDTO.Product.DescriptionEn,
                    //    DescriptionAr = productDTO.Product.DescriptionAr,
                    //    Flags = productDTO.Product.Flags,
                    //    Images = productDTO.Product.Images,
                    //    Subcategory = productDTO.Product.Subcategory,
                    //    SubcategoryId = productDTO.Product.SubcategoryId,
                    //    Price = productDTO.Product.Price,
                    //    PriceAfterDiscount = productDTO.Product.PriceAfterDiscount,
                    //    PersantageSale = productDTO.Product.PersantageSale
                    //};

                    var subproduct = await _db.Subproducts.Include(sp => sp.Product).Include(sp => sp.Image).SingleOrDefaultAsync(sp => sp.Id == subproductId);

                    // Attach the existing product instance if it's already being tracked
                    var trackedProduct = _db.Subproducts.Local.SingleOrDefault(p => p.Id == subproduct.Id);
                    if (trackedProduct != null)
                    {
                        _db.Entry(trackedProduct).State = EntityState.Detached;
                    }

                    _db.Attach(subproduct);

                    wishlistItem = new WishlistDetail
                    {
                        SubproductId = subproduct.Id,
                        WishlistId = wishlist.Id,
                        Quantity = qty,
                        Subproduct = subproduct,
                        Image = subproduct?.Image?.ImagePath,
                        Wishlist = wishlist,
                        UnitPrice = subproduct.Product?.Price ?? 0,
                    };
                    _db.WishlistDetail.Add(wishlistItem);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            await _db.SaveChangesAsync();
        }


        private string GetUserId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var user = httpContext?.User;

            return user?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<int> AddUnknownUserItem(int subproductId, Guid wishListGroup)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var wishList = await GetOrCreateWishlistForUnknownUser(wishListGroup);

                var subproduct = await _db.Subproducts.Include(sp => sp.Image).SingleOrDefaultAsync(sp => sp.Id == subproductId);
                if (subproduct is null)
                {
                    await transaction.DisposeAsync();
                    throw new Exception("Product was not found");
                }
                await UpdateWishlistItem(subproduct.Id, wishList, 1);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately (e.g., log or throw)
            }

            var count = await GetUnknownWishListItemCount(wishListGroup);
            return count;
        }

        public async Task<int> AddUnknownUserItem(int subproductId, int qty, Guid wishListGroup)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var wishList = await GetOrCreateWishlistForUnknownUser(wishListGroup);
                await UpdateWishlistItem(subproductId, wishList, qty);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately (e.g., log or throw)
            }

            var count = await GetUnknownWishListItemCount(wishListGroup);
            return count;
        }

        public async Task<int> RemoveUnknownWishListItem(int subproductId, Guid wishListGroup)
        {
            try
            {
                var wishList = await GetUnknownUserWishlist(wishListGroup) ?? throw new Exception("Cart not found");
                var wishListItem = wishList.WishlistsDetail.Where(a => a.SubproductId == subproductId).FirstOrDefault();

                if (wishListItem != null)
                {
                    if (wishListItem.Quantity > 1)
                        wishListItem.Quantity -= 1;
                    else
                        _db.WishlistDetail.Remove(wishListItem);
                }

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable To Remove Item");
            }

            var cartItemCount = await GetUnknownWishListItemCount(wishListGroup);
            return cartItemCount;
        }


        public async Task<int> GetUnknownWishListItemCount(Guid wishListGroup)
        {
            var wishlist = await GetUnknownUserWishlist(wishListGroup);
            return wishlist?.WishlistsDetail.Sum(d => d.Quantity) ?? 0;
        }
    }

}
