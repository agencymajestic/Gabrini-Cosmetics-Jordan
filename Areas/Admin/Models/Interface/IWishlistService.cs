namespace GabriniCosmetics.Areas.Admin.Models.Interface
{
    public interface IWishlistService
    {
        Task<int> AddItem(int subproductId);
        Task<int> AddItem(int subproductId, int qty);
        Task<int> AddUnknownUserItem(int subproductId, Guid wishListGroup);
        Task<int> AddUnknownUserItem(int subproductId, int qty, Guid wishListGroup);
        Task<int> RemoveItem(int subproductId);
        Task<int> RemoveUnknownWishListItem(int subproductId, Guid wishListGroup);
        Task<Wishlist> GetUserWishlist();
        Task<Wishlist> GetUnknownWishList(Guid wishListGroup);
        Task<IEnumerable<WishlistDetail>> GetWishlistProductByUserId(string userId);
        Task RemoveWishlistProducts(IEnumerable<WishlistDetail> wishlistProducts);
        Task<Wishlist> GetWishlist(string userId);
        Task<Wishlist> GetUnknownUserWishlist(Guid wishListGroup);
        Task<int> GetWishlistItemCount();
        Task<int> GetUnknownWishListItemCount(Guid wishListGroup);

    }
}
