using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WizardShopAPI.Models;

[Table("Product")]
public partial class Product
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [StringLength(300)]
    [Unicode(false)]
    public string Description { get; set; } = null!;

    public double Price { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Rating { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string? IdPhoto { get; set; }

    public int IdsKategory { get; set; }

    public int Popularity { get; set; }

    public int Quantity { get; set; }
}
