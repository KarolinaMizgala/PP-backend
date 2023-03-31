using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WizardShopAPI.Models;
using WizardShopAPI.Validators;

namespace WizardShopAPI.DTOs
{
    public class RegisterDto
    {
        [Required]
        [StringLength(30)]
        [Unicode(false)]
        public string Username { get; set; } = null!;

        [Required]
        [NameLikeValue]
        [StringLength(50)]
        [Unicode(false)]
        public string Name { get; set; } = null!;

        [Required]
        [NameLikeValue]
        [StringLength(50)]
        [Unicode(false)]
        public string Surname { get; set; } = null!;

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
