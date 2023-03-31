using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WizardShopAPI.Models;

[Table("User")]
public partial class User
{
    [Key]
    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(30)]
    [Unicode(false)]
    public string Username { get; set; } = null!;

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string Surname { get; set; } = null!;

    [Required]
    [StringLength(320)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(200)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [Required]
    [StringLength(150)]
    [Unicode(false)]
    public string PasswordSalt { get; set; } = null!;

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string Role { get; set; } = null!;
}
