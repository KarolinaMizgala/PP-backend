using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace WizardShopAPI.Models
{
    public class Order
    {
        public int OrderId { get; set; }
       
       
        public DateTime DateCreated { get; set; }
        public DateTime? DatePayment { get; set; }
        public DateTime? DateShipped { get; set; }
        public DateTime? DateDelivered { get; set; }
        public OrderState OrderState { get; set; }
      
        public double TotalPrice { get; set; }

        public int OrderDetailsId { get; set; }
        [AllowNull]
        public int? PaymentId { get; set; }
        [ForeignKey("PaymentId")]
        
        public virtual Payment Payment { get; set; }
        public OrderDetails OrderDetails { get; set; }
        public List<OrderItem> OrderItems { get; set; }
       

    }

    public enum OrderState
    {
        Open,
        Paid,
        Shipped,
        Delivered,
        Cancelled
    }
}
