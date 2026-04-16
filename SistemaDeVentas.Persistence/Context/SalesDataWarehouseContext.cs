using Microsoft.EntityFrameworkCore;
using SistemaDeVentas.Persistence.Entities.DataWareHouse.Dimensions;
using SistemaDeVentas.Persistence.Entities.DataWareHouse.Facts;

namespace SistemaDeVentas.Persistence.Context
{
    public class SalesDataWarehouseContext : DbContext
    {
        public SalesDataWarehouseContext(DbContextOptions<SalesDataWarehouseContext> options) : base(options) { }

        public DbSet<DimDate> Dates => Set<DimDate>();
        public DbSet<DimLocation> Locations => Set<DimLocation>();
        public DbSet<DimCustomer> Customers => Set<DimCustomer>();
        public DbSet<DimCategory> Categories => Set<DimCategory>();
        public DbSet<DimProduct> Products => Set<DimProduct>();
        public DbSet<DimOrderStatus> OrderStatuses => Set<DimOrderStatus>();
        public DbSet<FactSales> Sales => Set<FactSales>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // DimDate
            modelBuilder.Entity<DimDate>(entity =>
            {
                entity.ToTable("DimDate");
                entity.HasKey(e => e.DateKey);

                entity.Property(e => e.Date).HasColumnType("date");
                entity.Property(e => e.MonthName).HasMaxLength(15).IsRequired();
                entity.Property(e => e.DayName).HasMaxLength(15).IsRequired();
            });

            // DimLocation
            modelBuilder.Entity<DimLocation>(entity =>
            {
                entity.ToTable("DimLocation");
                entity.HasKey(e => e.LocationKey);

                entity.Property(e => e.Country).HasMaxLength(100).IsRequired();
                entity.Property(e => e.City).HasMaxLength(100).IsRequired();

                entity.HasIndex(e => new { e.Country, e.City }).IsUnique();
            });

            // DimCustomer
            modelBuilder.Entity<DimCustomer>(entity =>
            {
                entity.ToTable("DimCustomer");
                entity.HasKey(e => e.CustomerKey);

                entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.HasIndex(e => e.CustomerID_NaturalKey).IsUnique();

                entity.HasOne(e => e.Location)
                      .WithMany(l => l.Customers)
                      .HasForeignKey(e => e.LocationKey)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // DimCategory
            modelBuilder.Entity<DimCategory>(entity =>
            {
                entity.ToTable("DimCategory");
                entity.HasKey(e => e.CategoryKey);

                entity.Property(e => e.CategoryName).HasMaxLength(100).IsRequired();
                entity.HasIndex(e => e.CategoryName).IsUnique();
            });

            // DimProduct
            modelBuilder.Entity<DimProduct>(entity =>
            {
                entity.ToTable("DimProduct");
                entity.HasKey(e => e.ProductKey);

                entity.Property(e => e.ProductName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.ListPrice).HasColumnType("decimal(10,2)");

                entity.HasIndex(e => e.ProductID_NaturalKey).IsUnique();

                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(e => e.CategoryKey)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // DimOrderStatus
            modelBuilder.Entity<DimOrderStatus>(entity =>
            {
                entity.ToTable("DimOrderStatus");
                entity.HasKey(e => e.StatusKey);

                entity.Property(e => e.StatusName).HasMaxLength(20).IsRequired();
                entity.HasIndex(e => e.StatusName).IsUnique();
            });

            // FactSales
            modelBuilder.Entity<FactSales>(entity =>
            {
                entity.ToTable("FactSales");
                entity.HasKey(e => e.SalesKey);

                entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");
                entity.Property(e => e.SalesAmount).HasColumnType("decimal(12,2)");

                entity.HasOne(e => e.Date)
                      .WithMany(d => d.FactSales)
                      .HasForeignKey(e => e.DateKey)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Customer)
                      .WithMany(c => c.FactSales)
                      .HasForeignKey(e => e.CustomerKey)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Product)
                      .WithMany(p => p.FactSales)
                      .HasForeignKey(e => e.ProductKey)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Location)
                      .WithMany(l => l.FactSales)
                      .HasForeignKey(e => e.LocationKey)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Status)
                      .WithMany(s => s.FactSales)
                      .HasForeignKey(e => e.StatusKey)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}