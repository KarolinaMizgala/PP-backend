using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WizardShopAPI.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        [StringLength(320)]
        [Unicode(false)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(40)]
        [Unicode(false)]
        public string Password { get; set; } = null!;
    }
}
