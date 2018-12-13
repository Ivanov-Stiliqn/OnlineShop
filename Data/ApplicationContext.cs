using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Enums;

namespace Data
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Size> Sizes { get; set; }

        public DbSet<ProductSize> ProductSizes { get; set; }

        public DbSet<Report> Reports { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<UserInfo> UserInfos { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().HasMany(u => u.MyProducts).WithOne(p => p.Creator).HasForeignKey(p => p.CreatorId);

            builder.Entity<User>().HasOne(u => u.UserInfo).WithOne(u => u.User).HasForeignKey<UserInfo>(u => u.UserId);

            builder.Entity<User>().HasMany(u => u.PurchaseOrders).WithOne(o => o.Buyer).HasForeignKey(u => u.BuyerId);

            builder.Entity<User>().HasMany(u => u.SellOrders).WithOne(o => o.Seller).HasForeignKey(u => u.SellerId);

            builder.Entity<Product>().HasMany(p => p.Orders).WithOne(o => o.Product).HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ProductSize>().HasOne(ps => ps.Product).WithMany(p => p.Sizes)
                .HasForeignKey(ps => ps.ProductId);

            builder.Entity<ProductSize>().HasOne(ps => ps.Size).WithMany(s => s.Products)
                .HasForeignKey(ps => ps.SizeId);

            builder.Entity<ProductSize>().HasKey(ps => new {ps.ProductId, ps.SizeId});

            builder.Entity<Report>().HasOne(r => r.Reporter).WithMany(u => u.ReportsGiven)
                .HasForeignKey(r => r.ReporterId);

            builder.Entity<Report>().HasOne(r => r.ReportedUser).WithMany(u => u.Reports)
                .HasForeignKey(r => r.ReportedUserId);

            builder.Entity<ClothesSize>().HasBaseType<Size>();
            builder.Entity<ShoesSize>().HasBaseType<Size>();
        }
    }
}
