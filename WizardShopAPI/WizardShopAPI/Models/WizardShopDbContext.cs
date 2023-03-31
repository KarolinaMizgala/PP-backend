using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WizardShopAPI.Models;

public partial class WizardShopDbContext : DbContext
{
    public WizardShopDbContext()
    {
    }

    public WizardShopDbContext(DbContextOptions<WizardShopDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<User> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.Property(e => e.AdressId).ValueGeneratedNever();
            entity.Property(e => e.ZipCode).IsFixedLength();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
