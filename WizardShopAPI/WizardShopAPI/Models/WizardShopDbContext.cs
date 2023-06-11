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

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrdersItems { get; set; }

    public virtual DbSet<OrderDetails> OrderDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>().Property(e => e.ZipCode).IsFixedLength();
        ;

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

        modelBuilder.Entity<Order>();
        modelBuilder.Entity<OrderDetails>();
        modelBuilder.Entity<OrderItem>();

        modelBuilder.Entity<Review>(entity =>
        {
            entity.Property(e => e.ReviewId).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
