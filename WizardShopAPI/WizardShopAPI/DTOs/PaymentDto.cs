using Microsoft.Build.Framework;
using PayPal.Api;
using WizardShopAPI.Models;

namespace WizardShopAPI.DTOs
{
    public class PaymentDto
    {
      
 
    
        public string? CardNumber { get; set; }
        public string? ExpirationDate { get; set; }
        public decimal? CVC { get; set; }
        public string? NameOnCard { get; set; }
        public string? Country { get; set; }
        public string? ZIP { get; set; }

    }
}
