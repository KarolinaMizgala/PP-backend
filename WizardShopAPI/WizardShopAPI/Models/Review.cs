using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WizardShopAPI.Models;

[Table("Review")]
public partial class Review
{
    [Key]
    [Required]
    public int ReviewId { get; set; }

    [Required]
    public int ProductId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Title { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Description { get; set; }

    [Required]
    public int Rating { get; set; }

    public int? UserId { get; set; }
}
