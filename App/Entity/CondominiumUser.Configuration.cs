using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NextCondoApi.Entity;

public class CondominiumUserEntityTypeConfiguration : IEntityTypeConfiguration<CondominiumUser>
{
    public void Configure(EntityTypeBuilder<CondominiumUser> builder)
    {
        builder
            .HasKey(condo => new { condo.UserId, condo.RelationshipType, condo.CondominiumId });

        builder
            .HasOne(condoUser => condoUser.User)
            .WithMany(user => user.CondominiumList)
            .HasForeignKey(condoUser => condoUser.UserId)
            .IsRequired();

        builder
            .HasOne(condoUser => condoUser.Condominium)
            .WithMany(condo => condo.Members)
            .HasForeignKey(condoUser => condoUser.CondominiumId)
            .IsRequired();

        builder
            .Navigation(condoUser => condoUser.User)
            .IsRequired();

        builder
            .Navigation(condoUser => condoUser.Condominium)
            .IsRequired();
    }
}
