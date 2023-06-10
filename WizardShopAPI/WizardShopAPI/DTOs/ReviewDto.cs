using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WizardShopAPI.DTOs
{
    public class ReviewDto
    {
        [Required]
        public int ProductId { get; set; }

        public int? UserId { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? Title { get; set; }

        [StringLength(200)]
        [Unicode(false)]
        public string? Description { get; set; }

        [Required]
        public int Rating { get; set; }
    }
}
