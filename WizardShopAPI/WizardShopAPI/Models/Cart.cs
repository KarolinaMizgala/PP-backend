namespace WizardShopAPI.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public List<CartItem> CartProducts { get; set; }
    }
}
