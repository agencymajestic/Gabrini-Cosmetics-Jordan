using System.ComponentModel.DataAnnotations;

namespace GabriniCosmetics.Areas.Admin.Models
{
    public class Order
    {
        public int ID { get; set; }
        public string UserID { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Address { get; set; }


        [Display(Name = "Address 2")]
        public string? Address2 { get; set; }

        public string? Country { get; set; }
        public string City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public string Phone { get; set; }
        public string? Fax { get; set; }
        public string? FullLocation { get; set; }

        public string Timestamp { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStstus { get; set; }
        public Guid? OrderGroup { get; set; } = null;
        public decimal TotalDiscount { get; set; }

    }
}
