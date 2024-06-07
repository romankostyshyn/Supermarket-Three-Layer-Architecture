using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Data.Data
{
    public class TradeMarketDbContext : DbContext
    {
        public TradeMarketDbContext(DbContextOptions<TradeMarketDbContext> options) : base(options)
        {
        }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<ReceiptDetail> ReceiptsDetails { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ReceiptDetail>()
            .HasKey(rd => new { rd.ReceiptId, rd.ProductId });

            modelBuilder.Entity<Person>()
                .HasOne(p => p.Customer)
                .WithOne(c => c.Person)
                .HasForeignKey<Customer>(c => c.PersonId);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Receipts)
                .WithOne(r => r.Customer)
                .HasForeignKey(r => r.CustomerId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(pc => pc.Products)
                .HasForeignKey(p => p.ProductCategoryId);

            modelBuilder.Entity<Receipt>()
                .HasMany(r => r.ReceiptDetails)
                .WithOne(rd => rd.Receipt)
                .HasForeignKey(rd => rd.ReceiptId);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.ReceiptDetails)
                .WithOne(rd => rd.Product)
                .HasForeignKey(rd => rd.ProductId);

            var decimalProps = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => (System.Nullable.GetUnderlyingType(p.ClrType) ?? p.ClrType) == typeof(decimal));

            foreach (var property in decimalProps)
            {
                property.SetPrecision(18);
                property.SetScale(2);
            }
        }
    }
}
