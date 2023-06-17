using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WizardShopAPI.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        [Unicode(false)]
        public string Name { get; set; } = null!;

        [Required]
        [Unicode(false)]
        public string Description { get; set; } = null!;

        [Required]
        public double Price { get; set; }

        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string Rating { get; set; } = "No data";

        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int Popularity { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
