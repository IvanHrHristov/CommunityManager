using CommunityManager.Infrastructure.Data.Models;
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

        public DbSet<Product> Products { get; set; } = null!;

        public DbSet<Marketplace> Marketplaces { get; set; } = null!;

        public DbSet<Message> Messages { get; set; } = null!;

        public DbSet<Chatroom> Chatrooms { get; set; } = null!;

        public DbSet<Community> Communities { get; set; } = null!;

        public DbSet<CommunityMember> CommunityMember { get; set; } = null!;

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

            builder
                .Entity<Product>()
                .HasOne(p => p.Buyer)
                .WithMany(s => s.ProductsPurchased)
                .HasForeignKey(p => p.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Product>()
                .HasOne(p => p.Marketplace)
                .WithMany(m => m.Products)
                .HasForeignKey(p => p.MarketplaceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Message>()
                .HasOne(m => m.Chatroom)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatroomId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(s => s.Messages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Community>()
                .HasMany(c => c.Marketplaces)
                .WithOne(m => m.Community);

            builder
                .Entity<Community>()
                .HasMany(c => c.Chatrooms)
                .WithOne(m => m.Community);

            base.OnModelCreating(builder);
        }
    }
}