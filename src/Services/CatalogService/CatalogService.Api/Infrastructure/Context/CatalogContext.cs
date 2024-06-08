using Microsoft.EntityFrameworkCore;
using CatalogService.Api.Core.Domain;
using CatalogService.Api.Infrastructure.EntityConfigurations;

namespace CatalogService.Api.Infrastructure.Context
{
    public class CatalogContext : DbContext
    {
        public DbSet<CatalogBrand> CatalogBrands { get; set; }
        public DbSet<CatalogType> CatalogTypes { get; set; }
        public DbSet<CatalogItem> CatalogItems { get; set; }

        public CatalogContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfiguration(new CatalogBrandEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CatalogTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CatalogItemEntityTypeConfiguration());


            //modelBuilder.Entity<CatalogBrand>().ToTable("CatalogBrand", "catalog");
            //modelBuilder.Entity<CatalogType>().ToTable("CatalogType", "catalog");
            //modelBuilder.Entity<CatalogItem>().ToTable("CatalogItem", "catalog");
        }

    }
}
