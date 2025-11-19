using Microsoft.EntityFrameworkCore;
using OrderProcessing.Core.Domain;

namespace OrderProcessing.Infrastructure
{
    public class OrdersDbContext : DbContext
    {
        public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");

                entity.HasKey(o => o.Id);

                entity.Property(o => o.Id)
                      .ValueGeneratedNever();

                entity.Property(o => o.CustomerId)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(o => o.CreatedAtUtc)
                      .IsRequired();

                entity.Property(o => o.Status)
                      .IsRequired();

                entity.HasMany(o => o.Items)
                      .WithOne()                     
                      .HasForeignKey(i => i.OrderId) 
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItems");

                entity.HasKey(i => i.Id);

                entity.Property(i => i.Id)
                      .ValueGeneratedNever();

                entity.Property(i => i.ProductId)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(i => i.Quantity)
                      .IsRequired();

                entity.Property(i => i.UnitPrice)
                      .HasColumnType("decimal(18,2)");
            });
        }
    }
}
