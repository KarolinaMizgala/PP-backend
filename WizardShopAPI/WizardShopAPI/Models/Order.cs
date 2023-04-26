using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WizardShopAPI.Models
{
    public class Order
    {
        public int OrderId { get; set; }
       
       
        public DateTime DateCreated { get; set; }
        public OrderState OrderState { get; set; }
      
        public double TotalPrice { get; set; }

        public int OrderDetailsId { get; set; }
      
        public OrderDetails OrderDetails { get; set; }
        public List<OrderItem> OrderItems { get; set; }
       

    }

    public enum OrderState
    {
        Open,
        Confirmed,
        Complete,
        Cancelled,
        Shipped,
        Delivered,
        Ready,
        Pending,
        Delayed,
        Partial,
        Backorder
    }
}
