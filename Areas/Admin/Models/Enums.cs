namespace GabriniCosmetics.Areas.Admin.Models
{
    public class Enums
    {

    }
    public enum PaymentMethod
    {
        CashOnDelivery = 1 ,
        CreditCard = 2,

    }
    public enum OrderStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Cancelled = 4,

    }
}
