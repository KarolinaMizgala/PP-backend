using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WizardShopAPI.DTOs
{
    public class OrderDetailsDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Comment { get; set; }
        [Required]
        [StringLength(6)]
        public string ZipCode { get; set; } = null!;

        [Required]
        [StringLength(40)]
        [Unicode(false)]
        public string City { get; set; } = null!;

        [Required]
        [StringLength(70)]
        [Unicode(false)]
        public string Street { get; set; } = null!;

        [Required]
        public int HouseNumber { get; set; }

        public int? ApartmentNumber { get; set; }
        
    }
}
