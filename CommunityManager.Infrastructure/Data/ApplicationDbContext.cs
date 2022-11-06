﻿using CommunityManager.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CommunityManager.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Community> Communities { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<CommunityMember>()
                .HasKey(cm => new { cm.CommunityId, cm.ApplicationUserId });

            builder
                .Entity<Product>()
                .HasOne(p => p.Seller)
                .WithMany(s => s.ProductsSold)
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.
                Entity<Product>()
                .HasOne(p => p.Buyer)
                .WithMany(s => s.ProductsPurchased)
                .HasForeignKey(p => p.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
        }
    }
}