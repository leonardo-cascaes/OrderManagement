using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("products");

            builder.HasKey(product => product.Id);

            builder.Property(product => product.Id)
                .HasColumnName("id");

            builder.Property(product => product.Name)
                .HasColumnName("name")
                .IsRequired();

            builder.Property(product => product.Price)
                .HasColumnName("price")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(product => product.Stock)
                .HasColumnName("stock")
                .IsRequired();

            builder.Property(product => product.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(product => product.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired(false);
        }
    }
}
