namespace GabriniCosmetics.Areas.Admin.Models.CustomerInfo
{
    public class Address
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address1 { get; set; }
        public string? Address2 { get; set; }
        public string PhoneNumber { get; set; }
        public string? FaxNumber { get; set; }
    }
}
