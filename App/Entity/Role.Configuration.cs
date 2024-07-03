using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimplifyCondoApi.Entity;

public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
{
  public void Configure(EntityTypeBuilder<Role> builder)
  {
    builder
      .HasKey(u => u.Name);

    builder
      .HasMany(e => e.Users)
      .WithOne(e => e.Role)
      .HasForeignKey(e => e.RoleId)
      .IsRequired();

    builder
      .HasData(new { Name = "Tenant" }, new { Name = "Manager" });
  }
}