using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("orders");

            builder.HasKey(order => order.Id);

            builder.Property(order => order.Id)
                .HasColumnName("id");

            builder.Property(order => order.CustomerId)
                .HasColumnName("customer_id")
                .IsRequired();

            builder.Property(order => order.Status)
                .HasColumnName("status")
                .IsRequired();

            builder.Property(order => order.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(order => order.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired(false);

            builder.Ignore(order => order.Total);

            builder.Property(order => order.CustomerId)
                .HasColumnName("customer_id")
                .IsRequired();

            builder.HasOne<Customer>()
                .WithMany()
                .HasForeignKey(order => order.CustomerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(order => order.OrderItems)
                .WithOne()
                .HasForeignKey("OrderId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(order => order.OrderItems)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
