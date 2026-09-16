using JardiTips.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JardiTips.Infrastructure.Configurations;

public class CategoryLikeConfiguration : IEntityTypeConfiguration<CategoryLikeEntity>
{
    public void Configure(EntityTypeBuilder<CategoryLikeEntity> builder)
    {
        builder.ToTable("CategoryLikes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.CategoryId)
            .IsRequired();

        builder.HasIndex(x => new { x.CategoryId, x.UserId })
            .IsUnique();

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Likes)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.CategoryLikes)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
