namespace GabriniCosmetics.Areas.Admin.Models.ViewModel
{
    public class HomeVM
    {
        public int TotalCategory { get; set; }
        public int TotalProducts { get; set; }
        public int TotalAllItems { get; set; }
        public int TotalNewProducts { get; set; }
        public int TotalSaleProducts { get; set; }
        public int TotalFeatureProducts { get; set; }
        public int TotalPendingOrders { get; set; }
        public int TotalApproveOrders { get; set; }
        public int TotalRejectedOrders { get; set; }
        public int TotalPaid { get; set; }
        public int TotalUnPaid { get; set; }
        public int TotalSubCategory { get; set; }
        public int TotalancelledByUser { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}
