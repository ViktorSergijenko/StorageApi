﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StorageAPI.Context;

namespace StorageAPI.Migrations
{
    [DbContext(typeof(StorageContext))]
    partial class StorageContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("StorageAPI.Models.AdminLogTable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Action");

                    b.Property<DateTime>("Date");

                    b.Property<string>("UserName");

                    b.Property<string>("What");

                    b.Property<string>("Where");

                    b.HasKey("Id");

                    b.ToTable("AdminLogTable");
                });

            modelBuilder.Entity("StorageAPI.Models.Basket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasFilter("[UserId] IS NOT NULL");

                    b.ToTable("Baskets");
                });

            modelBuilder.Entity("StorageAPI.Models.Catalog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("BasketId");

                    b.Property<Guid>("CatalogNameId");

                    b.Property<decimal>("CurrentAmount");

                    b.Property<int>("MaximumAmount");

                    b.Property<int>("MinimumAmount");

                    b.Property<decimal>("ProductPrice");

                    b.Property<bool>("Type");

                    b.Property<Guid?>("WarehouseId");

                    b.HasKey("Id");

                    b.HasIndex("BasketId");

                    b.HasIndex("WarehouseId");

                    b.HasIndex("CatalogNameId", "WarehouseId")
                        .IsUnique()
                        .HasFilter("[WarehouseId] IS NOT NULL");

                    b.ToTable("CatalogDB");
                });

            modelBuilder.Entity("StorageAPI.Models.CatalogName", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Amount");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("CatalogNameDB");
                });

            modelBuilder.Entity("StorageAPI.Models.News", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AcceptedFix");

                    b.Property<string>("Author");

                    b.Property<string>("AuthorAcceptedFix");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTime?>("FixedDate");

                    b.Property<bool>("FixedProblem");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("ShortDescription")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<Guid>("WarehouseId");

                    b.HasKey("Id");

                    b.HasIndex("WarehouseId");

                    b.ToTable("NewsDB");
                });

            modelBuilder.Entity("StorageAPI.Models.NewsComment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<string>("Comment")
                        .IsRequired();

                    b.Property<DateTime>("Date");

                    b.Property<Guid>("NewsId");

                    b.HasKey("Id");

                    b.HasIndex("NewsId");

                    b.ToTable("NewsCommentDB");
                });

            modelBuilder.Entity("StorageAPI.Models.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CatalogId");

                    b.Property<string>("Name");

                    b.Property<decimal>("PricePerOne");

                    b.Property<string>("VendorCode");

                    b.HasKey("Id");

                    b.HasIndex("CatalogId");

                    b.ToTable("ProductDB");
                });

            modelBuilder.Entity("StorageAPI.Models.SimpleLogTable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Action");

                    b.Property<decimal>("Amount");

                    b.Property<DateTime>("Date");

                    b.Property<string>("Manually");

                    b.Property<string>("UserName");

                    b.Property<Guid>("WarehouseId");

                    b.Property<string>("What");

                    b.Property<string>("Where");

                    b.HasKey("Id");

                    b.HasIndex("WarehouseId");

                    b.ToTable("SimpleLogTableDB");
                });

            modelBuilder.Entity("StorageAPI.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FullName")
                        .IsRequired();

                    b.Property<bool>("HasAbilityToLoad");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("ReportsTo");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserId");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.Property<string>("WhoCreated");

                    b.HasKey("Id");

                    b.HasIndex("FullName")
                        .IsUnique();

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("ReportsTo");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("StorageAPI.Models.UserSettings", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("CanAddProductsManually");

                    b.Property<bool>("CanDeleteUsers");

                    b.Property<bool>("CanEditUserBaskets");

                    b.Property<bool>("CanEditUserInformation");

                    b.Property<bool>("CanEditUserPassword");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasFilter("[UserId] IS NOT NULL");

                    b.ToTable("UserSettingsDB");
                });

            modelBuilder.Entity("StorageAPI.Models.UserWarehouse", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<Guid>("WarehouseId");

                    b.Property<bool>("DoesUserHaveAbilityToSeeProductAmount");

                    b.HasKey("UserId", "WarehouseId");

                    b.HasIndex("WarehouseId");

                    b.ToTable("UserWarehouseDB");
                });

            modelBuilder.Entity("StorageAPI.Models.Warehouse", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .IsRequired();

                    b.Property<bool>("HasMinCatalogs");

                    b.Property<bool>("HasProblems");

                    b.Property<string>("ImageBase64");

                    b.Property<string>("Location")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("QrCodeBase64");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("Name", "Address")
                        .IsUnique();

                    b.ToTable("WarehouseDB");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("StorageAPI.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("StorageAPI.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("StorageAPI.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("StorageAPI.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StorageAPI.Models.Basket", b =>
                {
                    b.HasOne("StorageAPI.Models.User", "User")
                        .WithOne("Basket")
                        .HasForeignKey("StorageAPI.Models.Basket", "UserId");
                });

            modelBuilder.Entity("StorageAPI.Models.Catalog", b =>
                {
                    b.HasOne("StorageAPI.Models.Basket", "Basket")
                        .WithMany("Catalogs")
                        .HasForeignKey("BasketId");

                    b.HasOne("StorageAPI.Models.CatalogName", "Name")
                        .WithMany("CatalogList")
                        .HasForeignKey("CatalogNameId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("StorageAPI.Models.Warehouse", "Warehouse")
                        .WithMany("Catalogs")
                        .HasForeignKey("WarehouseId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("StorageAPI.Models.News", b =>
                {
                    b.HasOne("StorageAPI.Models.Warehouse", "Warehouse")
                        .WithMany("News")
                        .HasForeignKey("WarehouseId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StorageAPI.Models.NewsComment", b =>
                {
                    b.HasOne("StorageAPI.Models.News", "News")
                        .WithMany("NewsComments")
                        .HasForeignKey("NewsId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StorageAPI.Models.Product", b =>
                {
                    b.HasOne("StorageAPI.Models.Catalog", "Catalog")
                        .WithMany("Products")
                        .HasForeignKey("CatalogId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StorageAPI.Models.SimpleLogTable", b =>
                {
                    b.HasOne("StorageAPI.Models.Warehouse", "Warehouse")
                        .WithMany("WarehouseLogs")
                        .HasForeignKey("WarehouseId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("StorageAPI.Models.User", b =>
                {
                    b.HasOne("StorageAPI.Models.User", "Boss")
                        .WithMany()
                        .HasForeignKey("ReportsTo");

                    b.HasOne("StorageAPI.Models.User")
                        .WithMany("Employees")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("StorageAPI.Models.UserSettings", b =>
                {
                    b.HasOne("StorageAPI.Models.User", "User")
                        .WithOne("Settings")
                        .HasForeignKey("StorageAPI.Models.UserSettings", "UserId");
                });

            modelBuilder.Entity("StorageAPI.Models.UserWarehouse", b =>
                {
                    b.HasOne("StorageAPI.Models.User", "User")
                        .WithMany("UserWarehouse")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("StorageAPI.Models.Warehouse", "Warehouse")
                        .WithMany("UserWarehouse")
                        .HasForeignKey("WarehouseId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
