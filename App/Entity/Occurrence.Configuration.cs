
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NextCondoApi.Entity;

public class OccurrenceEntityTypeConfiguration : IEntityTypeConfiguration<Occurrence>
{
    public void Configure(EntityTypeBuilder<Occurrence> builder)
    {
        builder
            .HasKey(o => o.Id);

        builder
            .Property(o => o.Title)
            .HasMaxLength(255)
            .IsRequired();

        builder
            .Property(o => o.Description)
            .HasMaxLength(4000);

        builder
            .HasOne(o => o.Creator)
            .WithMany()
            .HasForeignKey(o => o.CreatorId)
            .IsRequired();

        builder
            .HasOne(o => o.Condominium)
            .WithMany()
            .HasForeignKey(o => o.CondominiumId)
            .IsRequired();

        builder
            .HasOne(o => o.OccurrenceType)
            .WithMany()
            .HasForeignKey(o => o.OccurrenceTypeId)
            .IsRequired();
    }
}