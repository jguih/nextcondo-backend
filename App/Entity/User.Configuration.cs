
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NextCondoApi.Entity;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .HasKey(user => user.Id);

        builder
            .Property(user => user.Email)
            .IsRequired();

        builder
            .Property(user => user.FullName)
            .IsRequired()
            .HasMaxLength(255);

        builder
            .Property(user => user.PasswordHash)
            .IsRequired();

        builder
            .Property(user => user.Phone)
            .HasMaxLength(30);

        builder
            .Property(user => user.IsEmailVerified)
            .HasDefaultValue(false);

        builder
            .Property(user => user.EmailVerifiedAt);

        builder
          .HasOne(user => user.Role)
          .WithMany()
          .HasForeignKey(user => user.RoleId)
          .IsRequired();

        builder
          .Navigation(user => user.Role)
          .AutoInclude();
    }
}