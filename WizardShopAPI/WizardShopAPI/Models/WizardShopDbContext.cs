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

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.Property(e => e.AdressId).ValueGeneratedNever();
            entity.Property(e => e.ZipCode).IsFixedLength();

            entity.HasOne(d => d.User).WithMany(p => p.Addresses).HasConstraintName("FK_Address_User");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
