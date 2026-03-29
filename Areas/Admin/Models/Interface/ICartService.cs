using NuGet.Configuration;

namespace GabriniCosmetics.Areas.Admin.Models.Interface
{
    public interface ICartService
    {
        public Task<int> AddItem(int subproductId);
        public Task<int> AddItem(int subproductId, int qty);
        public Task<int> AddUnknownUserItem(int subproductId, Guid cartGroup);
        public Task<int> AddUnknownUserItem(int subproductId, int qty, Guid cartGroup);
        public Task<int> RemoveItem(int subproductId);
        public Task<int> RemoveUnknownCartItem(int subproductId, Guid cartGroup);
        public Task<ShoppingCart> GetUserCart();
        public Task<ShoppingCart> GetUnknownCart(Guid cartGroup);
        public Task<ShoppingCart> GetCart(string userId);
        public Task<ShoppingCart> GetUnknownUserCart(Guid cartGroup);
        public Task<int> GetCartItemCount();
        public Task<int> GetUnknownCartItemCount(Guid cartGroup);
        public Task<IEnumerable<CartDetail>> GetCartProductByUserId(string userId);
        public Task RemoveCartProducts(IEnumerable<CartDetail> cartProduct);
        public Task<IEnumerable<CartDetail>> GetCartDetailsShoppingCartById(int cartId);

    }
}
