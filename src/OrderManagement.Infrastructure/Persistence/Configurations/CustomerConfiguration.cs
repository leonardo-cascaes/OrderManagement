using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("customers");

            builder.HasKey(customer => customer.Id);

            builder.Property(customer => customer.Id)
                .HasColumnName("id");

            builder.Property(customer => customer.Name)
                .HasColumnName("name")
                .IsRequired();

            builder.Property(customer => customer.Email)
                .HasColumnName("email")
                .IsRequired();

            builder.Property(customer => customer.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

            builder.Property(customer => customer.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired(false);
        }
    }
}
