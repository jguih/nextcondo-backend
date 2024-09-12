using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NextCondoApi.Entity;

public class CondominiumUserTypeConfiguration : IEntityTypeConfiguration<CondominiumUser>
{
    public void Configure(EntityTypeBuilder<CondominiumUser> builder)
    {
        builder
            .HasKey(condo => new { condo.UserId, condo.RelationshipType, condo.CondominiumId });

        builder
            .HasOne(condoUser => condoUser.User)
            .WithMany()
            .HasForeignKey(condoUser => condoUser.UserId);

        builder
            .HasOne(condoUser => condoUser.Condominium)
            .WithMany(condo => condo.Members)
            .HasForeignKey(condoUser => condoUser.CondominiumId);

        builder
            .Navigation(condoUser => condoUser.User)
            .AutoInclude();

        builder
            .Navigation(condoUser => condoUser.Condominium)
            .AutoInclude();
    }
}
