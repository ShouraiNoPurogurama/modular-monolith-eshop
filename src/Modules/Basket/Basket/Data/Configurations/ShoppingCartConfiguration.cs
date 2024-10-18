using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Basket.Data.Configurations;

public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
{
    public void Configure(EntityTypeBuilder<ShoppingCart> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.UserName)
            .IsUnique();
        
        builder.Property(e => e.UserName)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasMany(s => s.Items)
            .WithOne()
            .HasForeignKey(si => si.ShoppingCartId);
    }
}