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
          .HasData(
            new
            {
                Name = "Tenant",
                CreatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc)),
                UpdatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc))
            },
            new
            {
                Name = "Manager",
                CreatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc)),
                UpdatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc))
            }
            );
    }
}