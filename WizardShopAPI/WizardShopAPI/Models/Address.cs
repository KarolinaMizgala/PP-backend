using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WizardShopAPI.Models;

[Table("Address")]
public partial class Address
{
    [Key]
    [Required]
    public int AdressId { get; set; }

    [Required]
    public int UserId { get; set; }

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
