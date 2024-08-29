using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NextCondoApi.Entity;

public class CondominiumUserTypeConfiguration : IEntityTypeConfiguration<CondominiumUser>
{
    public void Configure(EntityTypeBuilder<CondominiumUser> builder)
    {
        builder
            .HasKey(e => new { e.UserId, e.RelationshipType, e.CondominiumId });

        builder
            .Navigation(condo => condo.User)
            .AutoInclude();

        builder
            .Navigation(condo => condo.Condominium)
            .AutoInclude();
    }
}
