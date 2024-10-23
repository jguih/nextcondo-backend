using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NextCondoApi.Entity;

public class CommonAreaEntityTypeConfiguration : IEntityTypeConfiguration<CommonArea>
{
    public void Configure(EntityTypeBuilder<CommonArea> builder)
    {
        builder
            .HasKey(area => area.Id);

        builder
            .Property(area => area.StartTime)
            .IsRequired();

        builder
            .Property(area => area.EndTime)
            .IsRequired();

        builder
            .Property(area => area.TimeInterval)
            .IsRequired();

        builder
            .HasOne(area => area.Type)
            .WithMany()
            .HasForeignKey(area => area.TypeId);

        builder
            .HasOne(area => area.Condominium)
            .WithMany()
            .HasForeignKey(area => area.CondominiumId);
    }
}