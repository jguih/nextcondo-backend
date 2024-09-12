using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NextCondoApi.Entity;

public class CondominiumEntityTypeConfiguration : IEntityTypeConfiguration<Condominium>
{
    public void Configure(EntityTypeBuilder<Condominium> builder)
    {
        builder
          .HasKey(condo => condo.Id);

        builder
            .Property(condo => condo.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder
            .Property(condo => condo.Description)
            .HasMaxLength(2000);

        builder.Property(condo => condo.CreatedAt);

        builder.Property(condo => condo.UpdatedAt);

        builder
            .HasOne(condo => condo.Owner)
            .WithMany()
            .HasForeignKey(condo => condo.OwnerId)
            .IsRequired();

        builder
            .Navigation(condo => condo.Owner)
            .AutoInclude();

        builder
            .Navigation(condo => condo.Members)
            .AutoInclude();
    }
}