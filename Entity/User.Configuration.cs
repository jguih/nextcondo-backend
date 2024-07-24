
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NextCondoApi.Entity;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
          .HasKey(e => e.Id);

        builder
          .Property(e => e.Email)
          .IsRequired();

        builder.Property(e => e.FullName).IsRequired().HasMaxLength(255);

        builder.Property(e => e.Phone).HasMaxLength(30);

        builder
          .HasOne(e => e.Role)
          .WithMany(e => e.Users)
          .HasForeignKey(e => new { e.Id, e.RoleId })
          .IsRequired();

        builder
          .Navigation(e => e.Role)
          .AutoInclude();
    }
}