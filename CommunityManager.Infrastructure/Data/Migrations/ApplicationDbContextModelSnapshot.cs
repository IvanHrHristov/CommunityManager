﻿// <auto-generated />
using System;
using CommunityManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CommunityManager.Infrastructure.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CommunityManager.Infrastructure.Data.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("CommunityManager.Infrastructure.Data.Models.Chatroom", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CommunityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("CommunityId");

                    b.ToTable("Chatrooms");
                });

            modelBuilder.Entity("CommunityManager.Infrastructure.Data.Models.ChatroomMember", b =>
                {
                    b.Property<Guid>("ChatroomId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ChatroomId", "ApplicationUserId");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("ChatroomsMembers");
                });

            modelBuilder.Entity("CommunityManager.Infrastructure.Data.Models.Community", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("AgeRestricted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatorId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("Communities");
                });

            modelBuilder.Entity("CommunityManager.Infrastructure.Data.Models.CommunityMember", b =>
                {
                    b.Property<Guid>("CommunityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("CommunityId", "ApplicationUserId");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("CommunityMember");
                });

            modelBuilder.Entity("CommunityManager.Infrastructure.Data.Models.Marketplace", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CommunityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("CommunityId");

                    b.ToTable("Marketplaces");
                });

            modelBuilder.Entity("CommunityManager.Infrastructure.Data.Models.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ChatroomId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("SenderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ChatroomId");

                    b.HasIndex("SenderId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("CommunityManager.Infrastructure.Data.Models.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BuyerId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<Guid>("MarketplaceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<byte[]>("Photo")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("PhotoLenght")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasPrecision(18, 2)
                        .HasColumnType("money");

                    b.Property<string>("SellerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("BuyerId");

                    b.HasIndex("MarketplaceId");

                    b.HasIndex("SellerId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("CommunityManager.Infrastructure.Data.Models.Chatroom", b =>
                {
                    b.HasOne("CommunityManager.Infrastructure.Data.Models.Community", "Community")
                        .WithMany("Chatrooms")
                        .HasForeignKey("CommunityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Community");
                });

            modelBuilder.Entity("CommunityManager.Infrastructure.Data.Models.ChatroomMember", b =>
                {
                    b.HasOne("CommunityManager.Infrastructure.Data.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("ChatroomsMembers")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CommunityManager.Infrastructure.Data.Models.Chatroom", "Chatroom")
                        .WithMany("ChatroomsMembers")
                        .HasForeignKey("ChatroomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApplicationUser");

                    b.Navigation("Chatroom");
                });

            modelBuilder.Entity("CommunityManager.Infrastructure.Data.Models.Community", b =>
                {
                    b.HasOne("CommunityManager.Infrastructure.Data.Models.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("CommunityManager.Infrastructure.Data.Models.CommunityMember", b =>
                {
                    b.HasOne("CommunityManager.Infrastructure.Data.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("CommunitiesMembers")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CommunityManager.Infrastructure.Data.Models.Community", "Community")
                        .WithMany("CommunitiesMembers")
                        .HasForeignKey("CommunityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApplicationUser");

                    b.Navigation("Community");
                });

            modelBuilder.Entity("CommunityManager.Infrastructure.Data.Models.Marketplace", b =>
                {
                    b.HasOne("CommunityManager.Infrastructure.Data.Models.Community", "Community")
                        .WithMany("Marketplaces")
                        .HasForeignKey("CommunityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Community");
                });

            modelBuilder.Entity("CommunityManager.Infrastructure.Data.Models.Message", b =>
                {
                    b.HasOne("CommunityManager.Infrastructure.Data.Models.Chatroom", "Chatroom")
                        .WithMany("Messages")
                        .HasForeignKey("ChatroomId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CommunityManager.Infrastructure.Data.Models.ApplicationUser", "Sender")
                        .WithMany("Messages")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Chatroom");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("CommunityManager.Infrastructure.Data.Models.Product", b =>
                {
                    b.HasOne("CommunityManager.Infrastructure.Data.Models.ApplicationUser", "Buyer")
                        .WithMany("ProductsPurchased")
                        .HasForeignKey("BuyerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("CommunityManager.Infrastructure.Data.Models.Marketplace", "Marketplace")
                        .WithMany("Products")
                        .HasForeignKey("MarketplaceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CommunityManager.Infrastructure.Data.Models.ApplicationUser", "Seller")
                        .WithMany("ProductsSold")
                        .HasForeignKey("SellerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Buyer");

                    b.Navigation("Marketplace");

                    b.Navigation("Seller");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("CommunityManager.Infrastructure.Data.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("CommunityManager.Infrastructure.Data.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CommunityManager.Infrastructure.Data.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("CommunityManager.Infrastructure.Data.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CommunityManager.Infrastructure.Data.Models.ApplicationUser", b =>
                {
                    b.Navigation("ChatroomsMembers");

                    b.Navigation("CommunitiesMembers");

                    b.Navigation("Messages");

                    b.Navigation("ProductsPurchased");

                    b.Navigation("ProductsSold");
                });

            modelBuilder.Entity("CommunityManager.Infrastructure.Data.Models.Chatroom", b =>
                {
                    b.Navigation("ChatroomsMembers");

                    b.Navigation("Messages");
                });

            modelBuilder.Entity("CommunityManager.Infrastructure.Data.Models.Community", b =>
                {
                    b.Navigation("Chatrooms");

                    b.Navigation("CommunitiesMembers");

                    b.Navigation("Marketplaces");
                });

            modelBuilder.Entity("CommunityManager.Infrastructure.Data.Models.Marketplace", b =>
                {
                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
