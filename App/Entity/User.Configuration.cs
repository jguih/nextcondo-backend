
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimplifyCondoApi.Entity;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder
      .HasKey(e => e.Id);

    builder
      .Property(e => e.Email)
      .IsRequired();

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