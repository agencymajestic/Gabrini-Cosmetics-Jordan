namespace GabriniCosmetics.Areas.Admin.Models
{
    using System;

    public class Discount
    {
        public int Id { get; set; }

        // The unique code for the discount, 8-digit long.
        public string Code { get; set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

        // The percentage of sale.
        public decimal Percentage { get; set; }

        // The start date for the discount validity.
        public DateTime ValidFrom { get; set; }

        // The end date for the discount validity.
        public DateTime ValidTo { get; set; }

        // Determine if the discount is active or expired.
        public bool IsActive => DateTime.Now >= ValidFrom && DateTime.Now <= ValidTo;
    }

}
