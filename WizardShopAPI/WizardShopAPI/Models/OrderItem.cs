using System.ComponentModel.DataAnnotations.Schema;

namespace WizardShopAPI.Models
{
    
    public class OrderItem
    {
        
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public int OrderId { get; set; }

    }
}
