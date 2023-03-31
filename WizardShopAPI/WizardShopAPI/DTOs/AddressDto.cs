using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WizardShopAPI.DTOs
{
    public class AddressDto
    {
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
