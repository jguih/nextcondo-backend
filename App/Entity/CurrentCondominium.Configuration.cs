using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NextCondoApi.Entity;

public class CurrentCondominiumEntityTypeConfiguration : IEntityTypeConfiguration<CurrentCondominium>
{
    public void Configure(EntityTypeBuilder<CurrentCondominium> builder)
    {
        builder
            .HasKey(currentCondo => new { currentCondo.UserId, currentCondo.CondominiumId });

        builder
            .HasIndex(currentCondo => new { currentCondo.UserId, currentCondo.CondominiumId });

        builder
            .HasOne(currentCondo => currentCondo.User)
            .WithOne()
            .HasForeignKey<CurrentCondominium>(currentCondo => currentCondo.UserId);

        builder
            .HasOne(currentCondo => currentCondo.Condominium)
            .WithMany()
            .HasForeignKey(currentCondo => currentCondo.CondominiumId);

        builder
            .Navigation(currentCondo => currentCondo.User)
            .IsRequired();

        builder
            .Navigation(currentCondo => currentCondo.Condominium)
            .IsRequired();
    }
}
