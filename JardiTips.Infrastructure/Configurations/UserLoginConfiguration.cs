using JardiTips.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JardiTips.Infrastructure.Configurations;

public class UserLoginConfiguration : IEntityTypeConfiguration<UserLoginEntity>
{
    public void Configure(EntityTypeBuilder<UserLoginEntity> builder)
    {
        builder.ToTable("UserLogins");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.LoginProvider)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.ProviderKey)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(x => new { x.LoginProvider, x.ProviderKey })
            .IsUnique();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Logins)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
