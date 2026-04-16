using Microsoft.EntityFrameworkCore;
using SistemaDeVentas.Persistence.Entities.Database;

namespace SistemaDeVentas.Persistence.Context
{
    public class SalesDbContext : DbContext
    {
        public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options) { }

        public DbSet<CountryDb> Countries => Set<CountryDb>();
        public DbSet<CityDb> Cities => Set<CityDb>();
        public DbSet<CategoryDb> Categories => Set<CategoryDb>();
        public DbSet<OrderStatusDb> Statuses => Set<OrderStatusDb>();
        public DbSet<CustomerDb> Customers => Set<CustomerDb>();
        public DbSet<ProductDb> Products => Set<ProductDb>();
        public DbSet<OrderDb> Orders => Set<OrderDb>();
        public DbSet<OrderDetailsDb> OrderDetails => Set<OrderDetailsDb>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CountryDb>(entity =>
            {
                entity.ToTable("Countries");
                entity.HasKey(e => e.CountryID);

                entity.Property(e => e.Country).HasMaxLength(100).IsRequired();
                entity.HasIndex(e => e.Country).IsUnique();
            });

            modelBuilder.Entity<CityDb>(entity =>
            {
                entity.ToTable("Cities");
                entity.HasKey(e => e.CityID);

                entity.Property(e => e.City).HasMaxLength(100).IsRequired();
                entity.HasIndex(e => new { e.City, e.CountryID }).IsUnique();

                entity.HasOne(e => e.Country)
                      .WithMany(c => c.Cities)
                      .HasForeignKey(e => e.CountryID);
            });

            modelBuilder.Entity<CategoryDb>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(e => e.CategoryID);

                entity.Property(e => e.Category).HasMaxLength(100).IsRequired();
                entity.HasIndex(e => e.Category).IsUnique();
            });

            modelBuilder.Entity<OrderStatusDb>(entity =>
            {
                entity.ToTable("OrderStatus");
                entity.HasKey(e => e.StatusID);

                entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
                entity.HasIndex(e => e.Status).IsUnique();
            });

            modelBuilder.Entity<CustomerDb>(entity =>
            {
                entity.ToTable("Customers");
                entity.HasKey(e => e.CustomerID);

                entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();

                entity.HasOne(e => e.City)
                      .WithMany(c => c.Customers)
                      .HasForeignKey(e => e.CityID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProductDb>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(e => e.ProductID);

                entity.Property(e => e.ProductName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");

                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(e => e.CategoryID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<OrderDb>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(e => e.OrderID);

                entity.Property(e => e.OrderDate).HasColumnType("date");

                entity.HasOne(e => e.Customer)
                      .WithMany(c => c.Orders)
                      .HasForeignKey(e => e.CustomerID);

                entity.HasOne(e => e.Status)
                      .WithMany(s => s.Orders)
                      .HasForeignKey(e => e.StatusID);
            });

            modelBuilder.Entity<OrderDetailsDb>(entity =>
            {
                entity.ToTable("OrderDetails");
                entity.HasKey(e => e.OrderDetailsID);

                entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");

                entity.HasOne(e => e.Order)
                      .WithMany(o => o.OrderDetails)
                      .HasForeignKey(e => e.OrderID);

                entity.HasOne(e => e.Product)
                      .WithMany(p => p.OrderDetails)
                      .HasForeignKey(e => e.ProductID);
            });
        }
    }
}