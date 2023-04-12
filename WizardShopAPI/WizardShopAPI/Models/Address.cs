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
    public int AdressId { get; set; }

    public int UserId { get; set; }

    [StringLength(6)]
    public string ZipCode { get; set; } = null!;

    [StringLength(40)]
    [Unicode(false)]
    public string City { get; set; } = null!;

    [StringLength(70)]
    [Unicode(false)]
    public string Street { get; set; } = null!;

    public int HouseNumber { get; set; }

    public int? ApartmentNumber { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Addresses")]
    public virtual User User { get; set; } = null!;
}
