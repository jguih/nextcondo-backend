using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NextCondoApi.Entity;

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
          .HasData(new { Name = "Tenant", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow }, new { Name = "Manager", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow });
    }
}