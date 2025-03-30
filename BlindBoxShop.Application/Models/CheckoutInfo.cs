namespace BlindBoxShop.Application.Models
{
    public class CheckoutInfo
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = "Cash on Delivery";
    }
} 