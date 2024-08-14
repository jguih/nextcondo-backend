
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
          .HasOne(user => user.Role)
          .WithMany(user => user.Users)
          .HasForeignKey(user => new { user.Id, user.RoleId })
          .IsRequired();

        builder
          .Navigation(user => user.Role)
          .AutoInclude();
    }
}