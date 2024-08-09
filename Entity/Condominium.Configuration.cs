using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NextCondoApi.Entity;

public class CondominiumEntityTypeConfiguration : IEntityTypeConfiguration<Condominium>
{
    public void Configure(EntityTypeBuilder<Condominium> builder)
    {
        builder
          .HasKey(o => o.Id);

        builder
            .Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder
            .Property(o => o.Description)
            .HasMaxLength(255);

        builder.Property(o => o.Created);

        builder.Property(o => o.Updated);

        builder
            .HasOne(condo => condo.Owner)
            .WithOne(user => user.Condominium)
            .HasForeignKey<Condominium>(condo => condo.OwnerId)
            .IsRequired();

        builder
            .Navigation(e => e.Owner)
            .AutoInclude();
    }
}