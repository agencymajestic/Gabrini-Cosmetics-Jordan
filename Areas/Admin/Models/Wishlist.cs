using GabriniCosmetics.Data;
using System.ComponentModel.DataAnnotations;

namespace GabriniCosmetics.Areas.Admin.Models
{
    public class Wishlist
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Guid? WishListGroup { get; set; } = null;

        public ICollection<WishlistDetail> WishlistsDetail { get; set; } = new List<WishlistDetail>();

    }
}
