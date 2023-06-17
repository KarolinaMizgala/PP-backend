using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WizardShopAPI.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        public PaymentType Type { get; set; }
        public string? CardNumber { get; set; }
        public string? ExpirationDate { get; set; }
        public decimal? CVC { get; set; }
        public string? NameOnCard { get; set; }
        public string? Country { get; set; }
        public string? ZIP { get; set; }
      

    }
    public enum PaymentType
    {
        PayPal,
        Card
    }
}
