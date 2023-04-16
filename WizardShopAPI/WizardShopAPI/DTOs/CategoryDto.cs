using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WizardShopAPI.DTOs
{
    public class CategoryDto
    {
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string Name { get; set; } = null!;

        [StringLength(100)]
        [Unicode(false)]
        public string? Description { get; set; }

        public int? Popularity { get; set; }
    }
}
