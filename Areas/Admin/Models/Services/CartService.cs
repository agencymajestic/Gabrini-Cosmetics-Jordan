using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;

namespace GabriniCosmetics.Areas.Admin.Models.Services
{
    public class CartService :  ICartService
    {
        private readonly GabriniCosmeticsContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(GabriniCosmeticsContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> AddItem(int subproductId)
        {
            string userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("User is not logged in");

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var cart = await GetOrCreateCart(userId);

                var cartItem = cart.CartDetails.SingleOrDefault(a => a.SubproductId == subproductId);
                if (cartItem != null)
                {
                    cartItem.Quantity += 1;
                }
                else
                {
                    var subproduct = await _db.Subproducts.Include(sp => sp.Product).Include(sp => sp.Image).SingleOrDefaultAsync(p => p.Id == subproductId);
                    if (subproduct is null)
                    {
                        await transaction.DisposeAsync();
                        throw new Exception("Product was not found");
                    }

                    cartItem = new CartDetail
                    {
                        SubproductId = subproduct.Id,
                        ShoppingCartId = cart.Id,
                        Quantity = 1,
                        Subproduct = subproduct,
                        Image = subproduct.Image.ImagePath,
                        ShoppingCart = cart,
                        UnitPrice = subproduct.Product.Price
                    };

                    _db.CartDetails.Add(cartItem);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Handle the exception appropriately (e.g., log or throw)
            }

            var cartItemCount = await GetCartItemCount();
            return cartItemCount;
        }
        public async Task<int> AddItem(int subproductId, int qty)
        {
            string userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("User is not logged in");

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                ShoppingCart cart = await GetOrCreateCart(userId);
               
                await UpdateCartItem(subproductId, cart, qty);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately (e.g., log or throw)
            }

            var cartItemCount = await GetCartItemCount();
            return cartItemCount;
        }

        public async Task<int> AddUnknownUserItem(int subproductId, Guid cartGroup)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var cart = await GetOrCreateCartForUnknownUser(cartGroup);

                var cartItem = cart.CartDetails.SingleOrDefault(a => a.SubproductId == subproductId);
                if (cartItem != null)
                {
                    cartItem.Quantity += 1;
                }
                else
                {
                    var subproduct = await _db.Subproducts.Include(sp => sp.Product).Include(sp => sp.Image).SingleOrDefaultAsync(p => p.Id == subproductId);
                    if (subproduct is null)
                    {
                        await transaction.DisposeAsync();
                        throw new Exception("Product was not found");
                    }

                    cartItem = new CartDetail
                    {
                        SubproductId = subproduct.Id,
                        ShoppingCartId = cart.Id,
                        Quantity = 1,
                        Subproduct = subproduct,
                        Image = subproduct.Image.ImagePath,
                        ShoppingCart = cart,
                        UnitPrice = subproduct.Product.Price
                    };

                    _db.CartDetails.Add(cartItem);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Unable To Create Cart");
            }

            var cartItemCount = await GetCartItemCount();
            return cartItemCount;
        }
        public async Task<int> AddUnknownUserItem(int subproductId, int qty, Guid cartGroup)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var cart = await GetOrCreateCartForUnknownUser(cartGroup);

                await UpdateCartItem(subproductId, cart, qty);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable To Create Cart");
            }

            var cartItemCount = await GetCartItemCount();
            return cartItemCount;
        }

        public async Task<int> RemoveItem(int subproductId)
        {
            string userId = GetUserId();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User is not logged in");

                var cart = await GetCart(userId);
                if (cart == null)
                    throw new Exception("Cart not found");

                //var cartItem = cart.CartDetails.FirstOrDefault(a => a.ProductId == bookId);
                var cartItem = cart.CartDetails.Where(a => a.SubproductId == subproductId).FirstOrDefault();

                if (cartItem != null)
                {
                    if (cartItem.Quantity > 1)
                        cartItem.Quantity -= 1;
                    else
                        _db.CartDetails.Remove(cartItem);
                }

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately (e.g., log or throw)
            }

            var cartItemCount = await GetCartItemCount();
            return cartItemCount;
        }

        public async Task<int> RemoveUnknownCartItem(int subproductId, Guid cartGroup)
        {
            try
            {
                var cart = await GetUnknownUserCart(cartGroup) ?? throw new Exception("Cart not found");
                var cartItem = cart.CartDetails.Where(a => a.SubproductId == subproductId).FirstOrDefault();

                if (cartItem != null)
                {
                    if (cartItem.Quantity > 1)
                        cartItem.Quantity -= 1;
                    else
                        _db.CartDetails.Remove(cartItem);
                }

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable To Remove Item");
            }

            var cartItemCount = await GetUnknownCartItemCount(cartGroup);
            return cartItemCount;
        }

        public async Task<ShoppingCart> GetUserCart()
        {
            string userId = GetUserId();
            if (userId == null)
            {
                // Handle the case where userId is null (e.g., redirect to login or show an error message)
                // For now, let's return an empty cart or some default response
                var emptyCart = new ShoppingCart(); // You need to replace this with your actual Cart class
                return emptyCart;
            }
            else
            {
                var cart = await GetOrCreateCart(userId);
                return cart;
            }
        }
        public async Task<ShoppingCart> GetUnknownCart(Guid cartGroup)
        {

            var cart = await GetOrCreateCartForUnknownUser(cartGroup);
            return cart;
        }

        public async Task<IEnumerable<CartDetail>> GetCartDetailsShoppingCartById(int cartId)
        {
            return await _db.CartDetails
                .Include(d => d.Subproduct).ThenInclude(sp => sp.Image)
                .Include(d => d.Subproduct).ThenInclude(sp => sp.Product).ThenInclude(p => p.Flags)
                .Include(d => d.Subproduct).ThenInclude(sp => sp.Product).ThenInclude(p => p.Subcategory).ThenInclude(sc => sc.Category)
                .Where(c => c.ShoppingCartId == cartId).ToListAsync();
        }

        public async Task<IEnumerable<CartDetail>> GetCartProductByUserId(string userId)
        {
            var cart = await GetCart(userId);
            if (cart is null)
                throw new Exception("Invalid cart");
            return cart.CartDetails.ToList();
        }
        public async Task RemoveCartProducts(IEnumerable<CartDetail> cartProduct)
        {
            _db.CartDetails.RemoveRange(cartProduct);
            await _db.SaveChangesAsync();
        }
        public async Task<ShoppingCart> GetCart(string userId)
        {
            return await _db.ShoppingCarts
                .Include(c => c.CartDetails).ThenInclude(d => d.Subproduct).ThenInclude(sp => sp.Image)
                .Include(c => c.CartDetails).ThenInclude(d => d.Subproduct).ThenInclude(sp => sp.Product).ThenInclude(p => p.Flags)
                .Include(c => c.CartDetails).ThenInclude(d => d.Subproduct).ThenInclude(sp => sp.Product).ThenInclude(p => p.Subcategory).ThenInclude(sc => sc.Category)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
        public async Task<ShoppingCart> GetUnknownUserCart(Guid cartGroup)
        {
            return await _db.ShoppingCarts
                .Include(c => c.CartDetails).ThenInclude(d => d.Subproduct).ThenInclude(sp => sp.Image)
                .Include(c => c.CartDetails).ThenInclude(d => d.Subproduct).ThenInclude(sp => sp.Product).ThenInclude(p => p.Flags)
                .Include(c => c.CartDetails).ThenInclude(d => d.Subproduct).ThenInclude(sp => sp.Product).ThenInclude(p => p.Subcategory).ThenInclude(sc => sc.Category)
                .FirstOrDefaultAsync(c => c.CartGroup == cartGroup);
        }

        public async Task<int> GetCartItemCount()
        {
            string userId = GetUserId();
            var cart = await GetCart(userId);
            return cart?.CartDetails.Sum(d => d.Quantity) ?? 0;
        }

        public async Task<int> GetUnknownCartItemCount(Guid cartGroup)
        {
            var cart = await GetUnknownUserCart(cartGroup);
            return cart?.CartDetails.Sum(d => d.Quantity) ?? 0;
        }

        private async Task<ShoppingCart> GetOrCreateCart(string userId)
        {
            var cart = await GetCart(userId);

            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    UserId = userId,
                    CartDetails = new List<CartDetail>()
                };
                _db.ShoppingCarts.Add(cart);
                await _db.SaveChangesAsync();
            }
            return cart;
        }
        private async Task<ShoppingCart> GetOrCreateCartForUnknownUser(Guid cartGroup)
        {
            ShoppingCart cart = await GetUnknownUserCart(cartGroup);

            if (cart is null)
            {
                var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == "unknown@GabriniCosmetics.com") ?? throw new Exception("Unable To Create Cart");
                cart = new ShoppingCart
                {
                    UserId = user.Id,
                    CartDetails = new List<CartDetail>(),
                    CartGroup = cartGroup,
                };
                _db.ShoppingCarts.Add(cart);
                await _db.SaveChangesAsync();
            }

            return cart;
        }

        private string GetUserId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var user = httpContext?.User;

            return user?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private async Task UpdateCartItem(int subproductId, ShoppingCart cart, int qty)
        {
            try
            {
                //if (string.IsNullOrEmpty(img))
                //{
                //    throw new ArgumentNullException(nameof(img), "Image URL cannot be null or empty.");
                //}

                var cartItem = cart.CartDetails.Where(c => c.SubproductId == subproductId).FirstOrDefault();
                if (cartItem != null)
                {
                    //if (cartItem.Image != img)
                    //{
                    //    var subproduct = await _db.Subproducts.Include(sp => sp.Product).SingleOrDefaultAsync(sp => sp.Id == subproductId);

                    //    // Detach the existing product instance if it's already being tracked
                    //    var trackedProduct = _db.Subproducts.Local.FirstOrDefault(sp => sp.Id == subproductId);
                    //    if (trackedProduct != null)
                    //    {
                    //        _db.Entry(trackedProduct).State = EntityState.Detached;
                    //    }

                    //    _db.Attach(subproduct);

                    //    cartItem = new CartDetail
                    //    {
                    //        SubproductId = subproductId,
                    //        ShoppingCartId = cart.Id,
                    //        Quantity = qty,
                    //        Image = subproduct.Image.ImagePath,
                    //        Subproduct = subproduct,
                    //        ShoppingCart = cart,
                    //        UnitPrice = subproduct.Product?.Price ?? 0,
                    //    };

                    //    _db.CartDetails.Add(cartItem);
                    //}
                    //else
                    //{
                        cartItem.Quantity += qty;
                    //}
                }
                else
                {
                    var subproduct = await _db.Subproducts.Include(sp => sp.Product).Include(sp => sp.Image).SingleOrDefaultAsync(sp => sp.Id == subproductId);

                    // Detach the existing product instance if it's already being tracked
                    var trackedProduct = _db.Subproducts.Local.FirstOrDefault(sp => sp.Id == subproductId);
                    if (trackedProduct != null)
                    {
                        _db.Entry(trackedProduct).State = EntityState.Detached;
                    }

                    _db.Attach(subproduct);

                    cartItem = new CartDetail
                    {
                        SubproductId = subproductId,
                        ShoppingCartId = cart.Id,
                        Quantity = qty,
                        Subproduct = subproduct,
                        Image = subproduct?.Image?.ImagePath,
                        ShoppingCart = cart,
                        UnitPrice = subproduct.Product?.Price ?? 0,
                    };

                    _db.CartDetails.Add(cartItem);
                }

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

    }
}
